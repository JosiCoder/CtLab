#-------------------------------------------------------------------------------
#- Copyright (C) 2016 Josi Coder
#-
#- This program is free software: you can redistribute it and/or modify it
#- under the terms of the GNU General Public License as published by the Free
#- Software Foundation, either version 3 of the License, or (at your option)
#- any later version.
#-
#- This program is distributed in the hope that it will be useful, but WITHOUT
#- ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
#- FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
#- more details.
#-
#- You should have received a copy of the GNU General Public License along with
#- this program. If not, see <http://www.gnu.org/licenses/>.
#---------------------------------------------------------------------------------

# Target files
NATIVE_TEST = testnative
MANAGED_TEST = testmanaged.exe
LIB_NAME = interop
LIB_FILE = lib$(LIB_NAME).so

# Source files
LIB_MOD_SRC_FILES = interop_spi.c
NATIVE_TEST_SRC = test.c
MANAGED_TEST_SRC = test.cs

# Derived files
LIB_MOD_OBJ_FILES = $(LIB_MOD_SRC_FILES:%.c=%.o)
DEP_SRC_FILES = $(NATIVE_TEST_SRC) $(LIB_MOD_SRC_FILES)

# Options
MCS = /usr/bin/mcs
CC = /usr/bin/gcc
COMMON_CFLAGS = -Wall -Werror
LIB_MOD_CFLAGS = -c $(COMMON_CFLAGS) -fpic
DEPENDENCY_FILE = depend

# Rules
# =====

all: lib $(NATIVE_TEST) $(MANAGED_TEST)

lib: $(LIB_FILE)

dep: $(DEPENDENCY_FILE)

$(NATIVE_TEST): $(LIB_FILE) $(NATIVE_TEST_SRC)
	$(CC) -L$(CURDIR) $(COMMON_CFLAGS) -o $(NATIVE_TEST) $(NATIVE_TEST_SRC) -l$(LIB_NAME)

$(MANAGED_TEST): $(LIB_FILE) $(MANAGED_TEST_SRC)
	$(MCS) -out:$(MANAGED_TEST) $(MANAGED_TEST_SRC)

$(LIB_FILE): $(LIB_MOD_OBJ_FILES)
	$(CC) -shared -o $@ $^

%.o: %.c
	$(CC) $(LIB_MOD_CFLAGS) $<

.PHONY: clean
clean:
	rm $(MANAGED_TEST) $(NATIVE_TEST) $(LIB_FILE) $(LIB_MOD_OBJ_FILES)

$(DEPENDENCY_FILE): $(DEP_SRC_FILES)
	$(CC) -MM $(DEP_SRC_FILES) > $(DEPENDENCY_FILE)

-include $(DEPENDFILE)
