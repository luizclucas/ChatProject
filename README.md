# ChatProject
  it's a multi-client-server using TCP/SOCKET in .net using some Design Patterns, it was developed for an interview test.

## Steps to run(Without Visual Studio):

1- Download dotnet core RunTime and SDK on:
https://dotnet.microsoft.com/download

2 - Enter in ChatProject.Server folder, open CMD or POWERSHELL so execute: dotnet publish. (A Folder similar this would be your output C:\Projetos\ChatProject\ChatProject\ChatProject.Client\bin\Debug\netcoreapp2.2\publish\)

3 - Enter on that folder said in prior step and execute: dotnet ChatProject.Server.dll (Now your server is running)

4 - Make the samething in step 2 but for ChatProject.ClientSide (the output file would be something like: C:\Projetos\ChatProject\ChatProject\ChatProject.Server\bin\Debug\netcoreapp2.2\publish\)

5 - Enter on that folder published in step4 to the ClientSide and execute: dotnet ChatProject.ClientSide.dll

#### When using visual studio, make sure you have .netcore 2.2 or newer installed, so just execute it.

### **** Chat Commands ****

/LU => List the users that are in your room.

/NAME {YourName} => To set your nickname to let you be able to talk with others.

/PR {Person} {Message} => Let you send private message to a person.

/P {Person} {Message} => Let you send public message to a person.
/CTR {RoomName} => Creating a room

/CR {RoomName} => Changing room

/LR => List rooms.

/HELP => Show all commands

/EXIT => Leave chat.


#### Otherwise you can run using visual studio and .net core needs to be the one saying "Compatible with Visual Studio".
