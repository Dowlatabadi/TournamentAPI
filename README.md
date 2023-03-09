# Tournament Application
This is an ASP.Net application using MSSQL for a Tournament Application that allows Administrators to create and manage Contests, Questions, and Answers, and Players to participate in the Contests.

## Features
The application provides the following features:

- Administrators can create channels to group contests.
- Administrators can create contests within channels.
- Each contest can have multiple questions and each question can have multiple options.
- Players can participate in contests by calling the Participate endpoint and lock their desired amount of points to be able to answer questions.
- Since at the time of creating contests and questions, they don't have the answers, Administrators will call the SetAnswer endpoint later.
- Upon realizing all correct answers, Administrators can draw the winners of contests based on predefined contest options.

## Technologies
The application is built using the following technologies:

ASP.Net for the web application framework
MSSQL for the database
C# for the programming language
## Installation
To install and run the application, follow these steps:

1- Clone the repository.
2- Open the project in Visual Studio.
3- Build the solution.
4- Update the connection string in the appsettings.json file to point to your MSSQL database.
5- Run the database migration using Entity Framework by running the following command in the Package Manager Console:
```sql
Update-Database
```
6- Run the application.
# API Endpoints
The application provides the following API endpoints (https://localhost:xxxx/swagger):

## Channels
- GET /channels - Get a list of all channels.
- GET /channels/{id} - Get a specific channel by ID.
- POST /channels - Create a new channel.
- PUT /channels/{id} - Update a channel.
- DELETE /channels/{id} - Delete a channel.
## Contests
- GET /contests - Get a list of all contests.
- GET /contests/{id} - Get a specific contest by ID.
- GET /contests/{id}/participations/ - Get All Participations of a specific contest by ID.
- POST /contests/{id}/participations - participate in contest specified by ID and set supplied options as answers.
- GET /contests/{id}/questions - Get All questions of a specific contest by ID.
- POST /contests/{id} - Add a question to a contest specific by ID.
- GET /contests/{id}/stat - Get statistics of a specific contest by ID.
- POST /contests/Clone - Clones a contest specific by ID.
- POST /contests/{id}/activate - sets a contest to active state contest
- POST /contests/{id}/deactivate - deactivates a contest
- POST /contests/{id}/draw - draws winners of a contest according to redefined win parameters.
- POST /contests/{id}/resetdraw - clears the draw resutls(drawn rank, reward, ..).
## Questions
- GET /questions/{id}/options - Get a list of all questions in a specified contest.
- POST /questions/{id}/options - Add a new option to specified question.
- POST /questions/{id}/options - set a option as the answer of a question (resolve a single question specified by its Id and in case of resolving all qs of a contest, resolve the contest).


## Winners
- GET /Account/{id}/participations - Get All participation of a specified account by Id and list of her answers.
