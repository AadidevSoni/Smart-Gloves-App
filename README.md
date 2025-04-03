# Smart-Gloves-App
An app for my hardware workshop project on smart gloves for elderly and disabled

SMART GLOVES

The project follows an iterative development model by effective communication between the ESP32, sensors, firebase and app.

SKILLS REQUIRED

Hardware and Embedded Systems

1. Connecting MAX30100 (Heart & O2 sensor) and flex sensors
2. Circuit Design and Prototyping   

Software Development
1. ESP32 Programming - Writing Arduino/C++ code 
2. Firebase Realtime Database management - Storing and retrieving sensor data 
3. Unity & C# Programming - Develop mobile app UI and logic
4. RestClient - Using RestClient in unity to communicate with Firebase 
5. Game Development Logic - For interactive interface in Unity

Cloud & Networking
     
1. Wifi & IoT Networking - Connecting ESP32 to internet
2. Firebase Authentication - Securely logging in the ESP32 & app users
3. Firebase real-time updates - Using Firebase polling for instant updates
4. JSON Data Handling - Sending structured data between ESP32 and Firebase

UI/UX Design

1. Unity UI Design - Creating a friendly and accessible use interface
2. Text and Notification System - Displaying emergency alerts on the app
3. Button Controls & Animations - Interacting with the glove’s features via Unity

FireBase Cloud Integration

Inorder to integrate ESP32 with firebase, we have to install Firebase ESP32 & ESP8266 Client Library by Mobizt which is a powerful library which lets the ESP32 and ESP8266 to communicate with Firebase realtime database, Firestore and Firebase storage. It works with JSON format and supports HTTP methods such as Put, Push and Get. 

Inorder to connect the ESP32 to the firebase, we need the Firebase API key which is present in the project settings which acts as an authentication for the ESP32 to access and modify data in the firebase. We also need to provide the WIFI SSID and password as the ESP32 requires a network connection to communicate with the firebase servers. It also requires the email id and password associated with the firebase account inorder to establish a secure connection and to authenticate the device. 

We have to create FBDO (firebase data object) which handles firebase data operation and AUTH (firebase authentication object) which stores authentication data and CONFIG (firebase configuration object) which stores firebase configuration data.

After connecting to the wifi, configure the firebase authentication and initialize the connection. After reading the sensor data, make a JSON object of all sensor data and send it to firebase realtime data where the data will be processed by the RestClient of the app which sends the data into the unity app.

![Untitled Project (2)](https://github.com/user-attachments/assets/1e25f74d-6aaf-4408-b688-2ad1ca984209)
Fig: Firebase keys and values

The data sent from the sensors of ESP32 gets stored in their respective keys which are created in the firebase realtime database. The data are stored and received in these keys whose directory should be passed in the codes of ESP32 and Unity. 

Unity App Development

Front-End Development

Unity was used to design the friendly and simple user UI  which includes the UI panel, buttons, input fields, notification pop ups, dropdown texts, scenes and text fields. This was made possible by the intricate and fairly complicated application of Unity which lets the developer communicate between different objects in different scenes. The development in unity is purely using C# which is a modern and strong object oriented language primarily used for game and app development. C# scripting was used to handle real-time updates, firebase integration and user interface.

<img width="1512" alt="Screenshot 2025-03-27 at 8 51 31 AM" src="https://github.com/user-attachments/assets/20e10ed6-f398-428f-9df6-719f52972ce6" />
Fig Unity Working Space(Game scene, Working scene, Hierarchy, Inspector)

Fig shows the basic working space of unity. The leftmost space is the game scene which represents how the app will look like for the user. The aspect ratio of the scene is completely customisable and this scene only runs if the play button is pressed on top. The space left of it is the working scene where the developer can design the app and add UI elements. The Hierarchy space consists of all the objects present in the scene and they follow parent-child relationship where all the UI elements are child of the canvas element. 
The space below the hierarchy is the project space which is where all codes, textures, assets, audio, images, scenes, and packages are stored. The rightmost space is the inspector which is where we can assign scripts, and adjust the position, size, rotation, color, properties and characteristics of any object in the scene.

![App](https://github.com/user-attachments/assets/fb24abe0-68cc-446e-88fb-29bb7e3c246a)
Fig APP INTERFACE (Caretaker, Login, Wearer)

The app consists of 3 different scenes. The first scene is the LOGIN scene which is where the user can enter their details and choose to be the wearer or caretaker from a dropdown text. Depending on the choice, the WEARER or the CARETAKER scene will open.

The caretaker scene consists of updated data of the wearer’s heart-rate, blood O2 concentration, temperature as well as abnormal movement detection made possible through an accelerometer attached to the gloves. The caretaker will receive notifications whenever any of the data is abnormal indicating the wearer needs help. The caretaker can also send messages to the wearer using buttons or by typing out their message which pops up as a notification to the wearer.

The wearer scene will get updates on when the caretaker will come once they receive the notification as well any message that the caretaker sends them. It also has the list of different hand signals and their corresponding meanings so that the wearer doesn’t forget the gestures.

Back-End Development

The Back-End of the project is handled by firebase which acts as a communication medium between the sensors connected to ESP32 and the app. The connection between the app and firebase is done by a strong unity library called RestClient(Proyecto26).

RestClient sends HTTP requests such as Get, Post, Put, Patch from unity to the firebase. It formats all the data from the app and sensors as a JSON and sends it over HTTP and receives responses accordingly. The basic work-flow looks like this:
Unity sends HTTP requests to firebase.
Firebase processes the HTTP request.
The firebase sends back data to unity.
Unity processes the data and updates the app accordingly.

![Untitled Project](https://github.com/user-attachments/assets/3c0ce8de-63f1-47a1-8026-6979b5e350d6)
Fig Integration of Firebase into Unity app 

Note: The above code snippet is not a working C# integration of firebase to unity but just gives the basic understanding of Get, Put and Push functionality of RestClient to firebase and how to implement them

Inorder to use the RestClient package in unity, we have to import the package asset from unity’s asset store. After importing the project, we have to use it in the code front Proyecto26 library which comes with the RestClient package. 

Inorder to know the firebase to communicate with, we provide the firebase URL which is present in the firebase realtime data page. The above snippet shows 3 methods in the RestClient:

Get method 

This method is used to get data from the firebase. The URL we provided is the directory to the message key inside the firebase which handles all the messages that the caretaker sends to the wearer. After getting the data, “.Then(response => {});” is the code that runs after getting the data. In this case, the new message is stored in a string for storing receivedMessage and then it is displayed as a pop-up on a text element in the pop-up panel. The data is stored into the receivedMessage string so that every sec, it can be compared to the data in the firebase and whenever a change is found, the message is again displayed to the wearer.

Put method

This method is used to Put data into the firebase. We create a class of StatusMessage and store the message to pass to the firebase as an object of the class using the constructor passing the string message. This is because we cannot directly pass a string to the firebase as firebase only accepts structured JSON data and string is not JSON data. We pass the statusMessage object to the status key in the firebase and it gets stored there and this data is then taken by the Get method in wearer scene and hence an effective communication between the wearer and caretaker is established.

Patch method

Patch method is used instead of Put method to update the key value without deleting the existing key. In the code snippet, this method was used to initialize all the wearer status values to 0 before starting the app to make sure all the firebase values are set to default before running the application. 
