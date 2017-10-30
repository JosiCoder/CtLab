//------------------------------------------------------------------------------
// Copyright (C) 2016 Josi Coder

// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.

// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.

// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------

#include <unistd.h>
#include <stdio.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <errno.h>
#include <string.h>
#include <linux/spi/spidev.h>

#include "interop.h"

#define SET_ERROR_STRING_AND_RETURN(str) { set_error_string(error_string_buffer, error_string_maxlen, str); return -1; }

//#define PRINT_DEBUG_ON 1
#ifdef PRINT_DEBUG_ON
#define PRINT_DEBUG(...) printf(__VA_ARGS__)
#else
#define PRINT_DEBUG(...)
#endif

static uint8_t spi_mode = SPI_CPHA | SPI_CPOL;
static uint8_t bits_per_word = 8;
static uint32_t speed_hz = 500000;
static uint16_t delay_usecs;

#ifdef PRINT_DEBUG_ON
static void print_buffer(const char *caption, uint8_t *buf, int len)
{
    printf("%s: ", caption);
    int i;
    for (i = 0; i < len; i++)
    {
        printf("%.2X ", buf[i]);
    }
    puts("");
}
#endif

static void set_error_string(char *error_string_buffer, int error_string_maxlen, const char *error_string)
{
    if (NULL != error_string_buffer && error_string_maxlen > 0)
    {
        strerror_r(errno, error_string_buffer, error_string_maxlen);
        strncat(error_string_buffer, ", ", error_string_maxlen-strlen(error_string_buffer)-1);
        strncat(error_string_buffer, error_string, error_string_maxlen-strlen(error_string_buffer)-1);
        error_string_buffer[error_string_maxlen-1] = '\0';
    }
}

int open_spi(const char *device,
    char *error_string_buffer, int error_string_maxlen)
{
    int fd, ret;

    fd = open(device, O_RDWR);
    if (fd < 0) SET_ERROR_STRING_AND_RETURN("can't open device");

    // spi mode

    ret = ioctl(fd, SPI_IOC_WR_MODE, &spi_mode);
    if (ret == -1) SET_ERROR_STRING_AND_RETURN("can't set spi mode");

    ret = ioctl(fd, SPI_IOC_RD_MODE, &spi_mode);
    if (ret == -1) SET_ERROR_STRING_AND_RETURN("can't get spi mode");

    // bits per word

    ret = ioctl(fd, SPI_IOC_WR_BITS_PER_WORD, &bits_per_word);
    if (ret == -1) SET_ERROR_STRING_AND_RETURN("can't set bits per word");

    ret = ioctl(fd, SPI_IOC_RD_BITS_PER_WORD, &bits_per_word);
    if (ret == -1) SET_ERROR_STRING_AND_RETURN("can't get bits per word");

    // maximum speed

    ret = ioctl(fd, SPI_IOC_WR_MAX_SPEED_HZ, &speed_hz);
    if (ret == -1) SET_ERROR_STRING_AND_RETURN("can't set max speed hz");

    ret = ioctl(fd, SPI_IOC_RD_MAX_SPEED_HZ, &speed_hz);
    if (ret == -1) SET_ERROR_STRING_AND_RETURN("can't get max speed hz");

    PRINT_DEBUG("spi %s opened\n", device);
    PRINT_DEBUG("  spi mode: %d\n", spi_mode);
    PRINT_DEBUG("  bits per word: %d\n", bits_per_word);
    PRINT_DEBUG("  max speed: %d Hz (%d KHz)\n", speed_hz, speed_hz/1000);

    return fd;
}

static int transfer(int fd, uint8_t *tx_buf, uint8_t *rx_buf, int len,
    char *error_string_buffer, int error_string_maxlen)
{
    #ifdef PRINT_DEBUG_ON
    print_buffer("MOSI", tx_buf, len);
    #endif
    
    struct spi_ioc_transfer tr =
    {
        .tx_buf = (unsigned long)tx_buf,
        .rx_buf = (unsigned long)rx_buf,
        .len = len,
        .delay_usecs = delay_usecs,
        .speed_hz = speed_hz,
        .bits_per_word = bits_per_word,
    };

    int ret;
    ret = ioctl(fd, SPI_IOC_MESSAGE(1), &tr);
    if (ret < 1) SET_ERROR_STRING_AND_RETURN("can't send spi message");

    #ifdef PRINT_DEBUG_ON
    print_buffer("MISO", rx_buf, len);
    #endif
    
    return 0;
}

static int close_spi(int fd,
    char *error_string_buffer, int error_string_maxlen)
{
    int ret;

    ret = close(fd);
    if (ret < 0) SET_ERROR_STRING_AND_RETURN("can't close device");

    PRINT_DEBUG("spi closed\n");
    
    return ret;
}

static int transfer_data(const char *device, uint8_t *tx_buf, uint8_t *rx_buf, int len,
    char *error_string_buffer, int error_string_maxlen)
{
    int fd, ret;

    fd = open_spi(device, error_string_buffer, error_string_maxlen);
    if (fd < 0) return -1;
    
    ret = transfer(fd, tx_buf, rx_buf, len, error_string_buffer, error_string_maxlen);
    if (ret < 0) return ret;
    
    ret = close_spi(fd, error_string_buffer, error_string_maxlen);
    if (ret < 0) return ret;
  
    return 0;
}

int transfer_spi_data(const char *device, uint8_t *tx_buf, uint8_t *rx_buf, int len,
    char *error_string_buffer, int error_string_maxlen)
{
    PRINT_DEBUG("--------------------\n");
    int ret = transfer_data(device, tx_buf, rx_buf, len, error_string_buffer, error_string_maxlen);
    
    PRINT_DEBUG("--------------------\n");
    return ret;
}
