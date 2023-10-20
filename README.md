# .NET/C# Projects Portfolio
This repository contains several .NET/C# projects written as a way to improve coding skills and get to know .NET technologies.
Ideas for the projects were taken from [here](https://dev.to/nerdjfpb/15-c-project-ideas-beginner-to-expert-with-tutorial-iio).

### List of the projects
- Number Guessing Game
- Notebook App
- Phone Book App
- Tic-Tac-Toe Game


## Description of the projects
### [Number Guessing Game](NumberGuessing)
A simple console application allowing to play a game of guessing random numbers.

The game has 5 difficulty levels differing in the range of numbers to guess, so the probability of a correct guess. The score is calculated as a percentage of correct guesses so far.

#### Technologies and elements worth noting
- `DoInRobust` method that invokes an action until no exception is met. It was used to ensure proper user's input.


### [Notebook App](NoteApp)
A WPF application that serves as a notebook.

You can read, add, edit and delete notes with titles and creation date. The notes are stored in a JSON file, updated after closing the app. 

#### Technologies and elements worth noting
- WPF (XAML code)
- JSON serialization


### [Phone Book App](PhoneBook)
A WPF application that serbes as a phone book with detailed contacts.

The application requires a dedicated MSSQL database PhoneBook with stored procedures `AddContact`, `UpdateContact` and `UpdateContactIcon`, and a single entity Contact containing the follwoing fields:
- Id (`int`) - primary key
- FirstName (`varchar`)
- LastName (`varchar`)
- PhoneNumber (`varchar`)
- EmailAddress (`varchar`)
- Company (`varchar`)
- Icon (`image`)
- Favourite (`bit`)
  
The repository contains codes necessary to create such a database.

In the application you can display, add, edit and delete the contacts in the database using a simple form. All the contacts are displayed in a list with their icon and name, in the alphabetic order, first favourite contactes then the others. You can filter the list by contact's name dynamically using a dedicated search box. 

The application also has a logging system implented that logs any changes in the database and potential errors occurred during its runtime. Message boxes are utilised to confirm any crucial changes in the database, as well as to display the results of those changes. 

#### Technologies and elements worth noting
- WPF (XAML code)
- MSSQL database connection
- Custom logging system
- Working with images, employing ImageBrush and ImageSource classes
- XAML triggers for TextBox placeholder and changin the properties of controls during mouse hovering
