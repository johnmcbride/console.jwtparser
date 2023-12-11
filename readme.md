# JWT CLI Parser .NET Global Tool

## Overview

A .NET global tool that enable you to decode and create JWTs from the command line

I there are other web bases tools out there like [jwt.io](https://www.jwt.io/) that do a great job at helping with decoding JWTs. The general purpose of this global tool is the ability to use it on the command line.

## Installation
you can install the tool as a global tool using the following .NET Commands
```
dotnet tool install --global console.jwtparser
```

## Instructions
Once you have the tool installed you can run by issuing the **jwtconsole** command


### Extract Command

Parameters
- --pretty Prints out a formatted and colorized JSON string
- -p Prints out a formatted and colorized JSON string

### Examples

jwtconsole extract eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

jwtconsole extract eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c --pretty

jwtconsole extract eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c -p

### Developer Dependencies
- System.IdentityModel.Tokens.Jwt
- Spectre.Console.Cli
- Spectre.Console.Json