# arduino_32u4_programmer
This program will allow you burn a firmware file (.HEX) onto an ATMega32u4 microcontroller if it has an arduino bootloader. 
Under the hood, AVRDUDE (program for downloading and uploading the on-chip memories of Microchip’s AVR microcontrollers) is used invoked to perform the actual update.
In an effort to reduce the number of external dependencies, the following version of AVRDUDE is embedded inside this application:

avrdude.exe: Version 6.3-20190619
* Copyright (c) 2000-2005 Brian Dean, http://www.bdmicro.com/
* Copyright (c) 2007-2014 Joerg Wunsch

# Usage
The graphical interface requires a few input parameters prior to programming
* Windows COM Port of the device (Dropdown menu, click Refresh "F5" to update the list).
* Path of the .hex firmware file (FilePicker Dialog)

If you have multiple COM ports in the list, you can identify the correct on using the following technique:
* Unplug the device from your computer
* Refresh and note which ports are available.
* Plug the device back into your computer
* Select the new COM port


# Supported Boards
* [Adafruit Feather 32u4 Radio (RFM69HCW)](https://learn.adafruit.com/adafruit-feather-32u4-radio-with-rfm69hcw-module)
  * [USB Driver (Windows 7)](https://learn.adafruit.com/adafruit-feather-32u4-radio-with-rfm69hcw-module/using-with-arduino-ide)
  * USB Driver included with Windows 10 


# Compile from Source Prerequisites
* Windows 7,10
* [Visual Studio Community (Free)](https://visualstudio.microsoft.com/vs/community/)
* [.NET Framework 4.8.0 (or later)](https://dotnet.microsoft.com/download/dotnet-framework/net48)

# Resources
* https://learn.adafruit.com/adafruit-feather-32u4-radio-with-rfm69hcw-module/overview
* https://github.com/avrdudes/avrdude

# Contact
* Author: sheldon.blackshire@celltracktech.com
* Company: https://celltracktech.com/
