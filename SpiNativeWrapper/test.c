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

#include <stdio.h>
#include "interop.h"

static const int receiveBufferSize = 32;
static const char *device = "/dev/spidev0.1";


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

int main(int argc, char *argv[])
{
    uint8_t receiveBuffer[receiveBufferSize];

    uint8_t sendData[] =
    {
        0x00, 0x55, 0x00, 0x44,
    };

    int transmissionLength = ARRAY_SIZE(sendData);
    
    if (transmissionLength > receiveBufferSize)
    {
        printf("SPI transmission length must not exeed receive buffer size.");
        return -1;
    }
    
    print_buffer("sending", sendData, transmissionLength);

    char error_string_buffer[100];
    int ret = transfer_spi_data(device, sendData, receiveBuffer, transmissionLength,
        error_string_buffer, sizeof(error_string_buffer));

    if (ret < 0)
    {
        printf("SPI transmission failed: %s\n", error_string_buffer);
        return -1;
    }
    
    print_buffer("received", receiveBuffer, transmissionLength);

    return 0;
}
