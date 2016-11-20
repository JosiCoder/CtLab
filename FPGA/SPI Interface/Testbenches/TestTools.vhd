--------------------------------------------------------------------------------
-- Provides some tools for the test benches.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.ALL;
use ieee.numeric_std.all;
 
package TestTools is

    -- Functions and Procedures
    
    procedure serialize_byte (sclk_period: time;
                              parallel_in: std_logic_vector(7 downto 0);
                              signal sclk: out std_logic;
                              signal serial_out: out std_logic);

    procedure serialize_longword (sclk_period: time;
                                  parallel_in: std_logic_vector(31 downto 0);
                                  signal sclk: out std_logic;
                                  signal serial_out: out std_logic);

    procedure deserialize_longword (sclk_period: time;
                                    signal serial_in: std_logic;
                                    signal sclk: out std_logic;
                                    signal parallel_out: out std_logic_vector(31 downto 0));

    procedure serialize_and_deserialize_longword (
                        sclk_period: time;
                        parallel_in: std_logic_vector(7 downto 0);
                        signal serial_in: std_logic;
                        signal sclk: out std_logic;
                        signal serial_out: out std_logic;
                        signal parallel_out: out std_logic_vector(31 downto 0));
end;


package body TestTools is

    -----------------------------------------------------------------
    -- Serializes a single byte and writes it to a serial signal.
    -- Also controls the according SCLK signal.
    -----------------------------------------------------------------
    procedure serialize_byte (sclk_period: time;
                              parallel_in: std_logic_vector(7 downto 0);
                              signal sclk: out std_logic;
                              signal serial_out: out std_logic)
    is
    begin
        for i in parallel_in'high downto 0 loop
            sclk <= '0';
            serial_out <= parallel_in(i);
            wait for sclk_period/2;
            sclk <= '1';
            wait for sclk_period/2;
        end loop;
    end procedure;


    -----------------------------------------------------------------
    -- Serializes a single longword and writes it to a serial signal.
    -- Also controls the according SCLK signal.
    -----------------------------------------------------------------
    procedure serialize_longword (sclk_period: time;
                                  parallel_in: std_logic_vector(31 downto 0);
                                  signal sclk: out std_logic;
                                  signal serial_out: out std_logic)
    is
    begin
        serialize_byte(sclk_period, parallel_in(31 downto 24), sclk, serial_out);
        serialize_byte(sclk_period, parallel_in(23 downto 16), sclk, serial_out);
        serialize_byte(sclk_period, parallel_in(15 downto 8), sclk, serial_out);
        serialize_byte(sclk_period, parallel_in(7 downto 0), sclk, serial_out);
    end procedure;


    -----------------------------------------------------------------
    -- Deserializes a single longword by reading from a serial signal.
    -- Writes the deserialized data to the specified parallel signal
    -- continuously. Also controls the according SCLK signal.
    -----------------------------------------------------------------
    procedure deserialize_longword (sclk_period: time;
                                    signal serial_in: std_logic;
                                    signal sclk: out std_logic;
                                    signal parallel_out: out std_logic_vector(31 downto 0))
    is
    begin
        parallel_out <= (others => '0');
        for i in parallel_out'high downto 0 loop
            sclk <= '0';
            wait for sclk_period/2;
            parallel_out(i) <= serial_in;
            sclk <= '1';
            wait for sclk_period/2;
        end loop;
    end procedure;
    

    -----------------------------------------------------------------
    -- Serializes a single longword and writes it to a serial signal,
    -- simultaneously deserializes a single longword by reading from
    -- a serial signal. Writes the deserialized data to the specified
    -- parallel signal continuously.
    -- Also controls the according SCLK signal.
    -----------------------------------------------------------------
    procedure serialize_and_deserialize_longword (
                        sclk_period: time;
                        parallel_in: std_logic_vector(31 downto 0);
                        signal serial_in: std_logic;
                        signal sclk: out std_logic;
                        signal serial_out: out std_logic;
                        signal parallel_out: out std_logic_vector(31 downto 0))
    is
    begin
        parallel_out <= (others => '0');
        for i in parallel_in'high downto 0 loop
            sclk <= '0';
            serial_out <= parallel_in(i);
            wait for sclk_period/2;
            parallel_out(i) <= serial_in;
            sclk <= '1';
            wait for sclk_period/2;
        end loop;
    end procedure;

end;
