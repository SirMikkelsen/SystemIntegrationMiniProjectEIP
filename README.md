# Mini project EIP System Integration
In this projeect we are supposed to implement enterprise integration patterns so we can connect 2 projects.
We decided to use the project from large systems and integrate it so we can have many different hotels and hotel chains in this way.

## Get it working
In this, there are 2 different projects. In order to get them to work together, you need to set up RabbitMQ.

## Output
The sender takes a CSV file with information about the hotel and bookings and send via RabbitMQ to the receiver.
The receiver take the CSV file, convert it to XML, and save that, and put the information into the database.
