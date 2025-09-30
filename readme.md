# Password Manager (C#)

This is a project for the *Introduction to Programming* course.  
It is a simple **password manager application** that stores passwords securely in an encrypted text file.

# Features
- Master password protection at startup
- Add new passwords (stored using AES encryption)
- View all saved passwords (decrypted for display)
- Search for passwords by service name
- Strong password generator (cryptographically secure)

# Technologies
- C#
- .NET 6
- AES (Advanced Encryption Standard) in CBC mode
- PBKDF2 (Rfc2898DeriveBytes) for key derivation
- RandomNumberGenerator for password generation

# File Structure
- `Program.cs` – main source code
- `passwords.txt` – encrypted password storage
- `README.md` – project documentation

# Usage
1. Clone the project from GitHub
2. Open the folder in Visual Studio Code
3. Run the program:
   dotnet run

Enter the master password ('salainen' by default)

Choose an option from the menu:

Add a password

Show all saved passwords

Search for a password

Generate a strong password

Exit the program

Notes

This program was built as a course project.
For real-world use, it would require additional improvements such as:

Secure storage of the master password

Unique salt and IV per entry

Error handling and input validation

A graphical user interface