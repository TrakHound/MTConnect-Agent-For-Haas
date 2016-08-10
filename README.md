# MTConnect® Agent For Haas
Open Source MTConnect® Agent and Adapter for Haas CNC Machines using RS-232

This supports the basic data collection variables available using setting 143 on Haas machines described in the link below:

http://diy.haascnc.com/data-collection-using-rs-232-port-0

#### TODO:
- Add the ability to add Q600 variables to the configuration file
- Better error management


### Configuration File
```xml
<?xml version="1.0" encoding="utf-8" ?>
<Configuration>
  <Device>
  
	<!-- Device Name used in Devices.xml file for MTConnect® Agent's schema -->
    <DeviceName>Haas_Device</DeviceName>
	
	<!-- Port to communicate with MTConnect Agent -->
    <Port>7885</Port>
	
	<!-- Adapter heartbeat -->
    <Heartbeat>1000</Heartbeat>

	<!-- COM Port to communicate with the Haas machine -->
    <COMPort>COM3</COMPort>
	
  </Device>
</Configuration>
```
