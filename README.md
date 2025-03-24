# WPF Calculator Application

## Overview

This WPF-based calculator application, built in C#, replicates the standard Windows Calculator functionality while incorporating additional features from the Programmer Mode. It supports arithmetic operations, memory functions, digit grouping, and base conversions.

## Features

### Standard Arithmetic Operations: 
●Addition (+)

●Subtraction (-) 

●Multiplication (*) 

●Division (/) 

●Percentage (%) 

●Square Root (√)

●Square (x²)

●Reciprocal (1/x)

●Negation (+/-).

### Memory Functions:
●MC (Memory Clear)

●MR (Memory Recall)

●MS (Memory Store)

●M+ (Memory Add)

●M- (Memory Subtract)

●M> (Memory Stack Display)

### Utility Features:

●Backspace, Clear Entry (CE), Clear (C)

●Supports both Mouse & Keyboard Input

●Cut, Copy, Paste (Implemented via string manipulation)

●Digit Grouping (Formatted based on system locale settings)

### Programmer Mode:

●Supports base conversions: Binary (2), Octal (8), Decimal (10), Hexadecimal (16)

### Smart Persistence:

●Remembers Digit Grouping settings

●Restores last-used Calculator Mode (Standard/Programmer)

●Saves the Last Used Base in Programmer Mode

### Help & About Section: Displays developer details

### Optional Operator Precedence Mode: Execute operations according to mathematical precedence rules

## Installation

Clone the repository:

`git clone https://github.com/al3ssVil/Calculator.git`

Open the project in Visual Studio.

Build and run the application.

## Usage

Enter numbers and operations using the on-screen buttons or keyboard.

Click "=" or press Enter to evaluate expressions.

Press ESC to reset the calculator.

Memory functions allow storage and retrieval of values.

Enable/disable Digit Grouping from the menu.

Switch between Standard & Programmer mode easily.

Access Help > About for developer details.

## Persistence

The application saves user preferences between sessions:

Digit Grouping state

Last selected mode (Standard/Programmer)

Last used base in Programmer Mode

## Error Handling

Prevents division by zero.

Handles invalid input gracefully.

Displays error messages for unsupported operations.

## Future Improvements

Additional Scientific Functions (sin, cos, log, etc.)

UI Enhancements for a modern look & feel.

Localization Support for multiple languages.
