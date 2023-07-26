# Scada
 This project involves the automation of a sliding gate, aiming to implement control and monitoring of the gate using digital and analog sensors/inputs and buttons/outputs. The system utilizes TCP communication and the Modbus protocol for data exchange between the gate and the control system.

 Description of the system:

The system controls a sliding gate using buttons for opening and closing (Open and Close), and the gate's position (L) is measured as an analog output representing the distance of the gate from the starting wall (W1).
There are boundary values for the gate's position (LowAlarm and HighAlarm), where crossing these values triggers alarms.
The obstacle indicator (S) is a digital input that detects obstacles between the walls.
The system also uses analog outputs to display engineering units, where scaling and conversion of raw values to engineering units are performed.

Tasks assigned for this project include:

Configuring communication parameters for the TCP connection between the gate and the control system.
Setting up the "RtuCfg.txt" file defining all digital and analog inputs/outputs and their values, including parameters for alarms and conversions.
Periodically reading and updating the values of all digital and analog inputs/outputs on the user interface.
Enabling control through the control window for buttons and analog outputs, with the conversion of engineering units to raw values during command issuance.
Reporting HighAlarm and LowAlarm states of the gate when it crosses the corresponding boundary values.
Automatically stopping the gate and opening it back to the LowAlarm value if an obstacle is detected during closing.
