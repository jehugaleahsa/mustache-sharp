# mustache#

An extension of the mustache text template engine for .NET.

Download using NuGet: [mustache#](http://nuget.org/packages/mustache-sharp)

## Overview
Generating text has always been a chore. Either you're concatenating strings like a mad man or you're getting fancy with `StringBuilder`. Either way, the logic for conditionally including values or looping over a collection really obscures the intention of the code. A more declarative approach would improve your code big time. Hey, that's why server-side scripting got popular in the first place, right?

[mustache](http://mustache.github.com/) is a really simple tool for generating text. .NET developers already had access to `String.Format` to accomplish pretty much the same thing. The only problem was that `String.Format` used indexes for placeholders: `Hello, {0}!!!`. **mustache** let you use meaningful names for placeholders: `Hello, {{name}}!!!`.

**mustache** is a logic-less text generator. However, almost every time I've ever needed to generate text, I needed to turn some of it on or off, depending on a value. Not having the ability to turn things off usually meant going back to building my text in parts. 

Introducing [handlebars.js](http://handlebarsjs.com/). If you've needed to generate any HTML templates, **handlebars.js** is a really awesome tool. Not only does it support an `if` and `each` tag, it lets you define your own tags! It also makes it easy to reference nested values `{{Customer.Address.ZipCode}}`.

**mustache#** brings the power of **handlebars.js** to .NET and then takes it a little bit further. Not only does it support the same tags, it also handles whitespace intelligently. **mustache#** will automatically remove lines that contain nothing but whitespace and tags. This allows you to make text templates that are easy to read.

    Hello, {{Customer.Name}}
    
    {{#with Order}}
    {{#if LineItems}}
    Here is a summary of your previous order:
    
    {{#each LineItems}}
        {{ProductName}}: {{UnitPrice:C}} x {{Quantity}}
    {{/each}}
    
    Your total was {{Total:C}}.
    {{#else}}
    You do not have any recent purchases.
    {{/if}}
    {{/with}}
    
Most of the lines in the previous example will never appear in the final output. This allows you to use **mustache#** to write templates for normal text, not just HTML/XML.

## Placeholders
The placeholders can be any valid identifier. These map to the property names in your classes.

### Formatting Placeholders
Each format item takes the following form and consists of the following components:

    {{identifier[,alignment][:formatString]}}

The matching braces are required. The alignment and the format strings are optional and match the syntax accepted by `String.Format`. Refer to [String.Format](http://msdn.microsoft.com/en-us/library/system.string.format.aspx)'s documentation to learn more about the standard and custom format strings.

## The 'if' tag
## The 'each' tag
## The 'with' tag
