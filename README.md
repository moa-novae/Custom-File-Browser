# Pulsenics Technical Assessment

## Q1
All criteria of Q1 has been met. WPF is used to build the front end of the application. Squirrel is used to build and deploy the WPF application. Entity Framework Core is used as an O/RM to persist data in a SQLite database.
Windows service is used to sync the database when changes are made to the directory structure while the WPF application is closed.
Code to the WPF application can be found in Q1. Code to classes interacting with entity can be found in Q1Entity. The windows service can be found in Q1Service.

Path of the monitored directory can be changed in the app.config of Q1. The default path is C:\WpfTest

To run the WPF application, navigate to the release folder. You must first run Setup.msi, and then Setup.exe.

## Q2 & Q3

Answers and tests to solutions of Q2 and Q3 can be found in the folders Q2-3 and Q2-3Test.


