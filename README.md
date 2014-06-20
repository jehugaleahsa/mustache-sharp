# mustache#

An extension of the mustache text template engine for .NET.

Download using NuGet: [mustache#](http://nuget.org/packages/mustache-sharp)

## Overview
Generating text has always been a chore. Either you're concatenating strings like a mad man or you're getting fancy with `StringBuilder`. Either way, the logic for conditionally including values or looping over a collection really obscures the intention of the code. A more declarative approach would improve your code big time. Hey, that's why server-side scripting got popular in the first place, right?

[mustache](http://mustache.github.com/) is a really simple tool for generating text. .NET developers already had access to `String.Format` to accomplish pretty much the same thing. The only problem was that `String.Format` used indexes for placeholders: `Hello, {0}!!!`. **mustache** let you use meaningful names for placeholders: `Hello, {{name}}!!!`.

**mustache** is a logic-less text generator. However, almost every time I've ever needed to generate text I needed to turn some of it on or off depending on a value. Not having the ability to turn things off usually meant going back to building my text in parts.

Introducing [handlebars.js](http://handlebarsjs.com/)... If you've needed to generate any HTML templates, **handlebars.js** is a really awesome tool. Not only does it support an `if` and `each` tag, it lets you define your own tags! It also makes it easy to reference nested values `{{Customer.Address.ZipCode}}`.

**mustache#** brings the power of **handlebars.js** to .NET and then takes it a little bit further. It is geared towards building ordinary text documents, rather than just HTML. It differs from **handlebars.js** in the way it handles newlines. With **mustache#**, you explicitly indicate when you want newlines - actual newlines are ignored.

    Hello, {{Customer.Name}}
    {{#newline}}
    {{#newline}}
    {{#with Order}}
    {{#if LineItems}}
    Here is a summary of your previous order:
    {{#newline}}
    {{#newline}}
    {{#each LineItems}}
        {{ProductName}}: {{UnitPrice:C}} x {{Quantity}}
        {{#newline}}
    {{/each}}
    {{#newline}}
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

The matching braces are required. Notice that they are double curly braces! The alignment and the format strings are optional and match the syntax accepted by `String.Format`. Refer to [String.Format](http://msdn.microsoft.com/en-us/library/system.string.format.aspx)'s documentation to learn more about the standard and custom format strings.

### Placeholder Scope
The identifier is used to find a property with a matching name. If you want to print out the object itself, you can use the special identifier `this`.

    FormatCompiler compiler = new FormatCompiler();
    Generator generator = compiler.Compile("Hello, {{this}}!!!");
    string result = generator.Render("Bob");
    Console.Out.WriteLine(result);  // Hello, Bob!!!
    
Some tags, such as `each` and `with`, change which object the values will be retrieved from.

If a property with the placeholder name can't be found at the current scope, the name will be searched for at the next highest level.

**mustache#** will automatically detect when an object is a dictionary and search for a matching key. In this case, it still needs to be a valid identifier name.

### Nested Placeholders
If you want to grab a nested property, you can separate identifiers using `.`.

    {{Customer.Address.ZipCode}}

## The 'if' tag
The **if** tag allows you to conditionally include a block of text.

    Hello{{#if Name}}, {{Name}}{{/if}}!!!

The block will be printed if:
* The value is a non-empty string.
* The value is a non-empty collection.
* The value isn't the NUL char.
* The value is a non-zero number.
* The value evaluates to true.

The **if** tag has complimentary **elif** and **else** tags. There can be as many **elif** tags as desired but the **else** tag must appear only once and after all other tags.

    {{#if Male}}Mr.{{#elif Married}}Mrs.{{#else}}Ms.{{/if}}

## The 'each' tag
If you need to print out a block of text for each item in a collection, use the **each** tag.

    {{#each Customers}}
    Hello, {{Name}}!!
    {{/each}}
    
Within the context of the **each** block, the scope changes to the current item. So, in the example above, `Name` would refer to a property in the `Customer` class.

Additionally, you can access the current index into the collection being enumerated using the **index** tag.

    <ul>
    {{#each Items}}
        <li class="list-item{{#index}}" value="{{Value}}">{{Description}}</li>
    {{/each}}
    </ul>
    
This will build an HTML list, building a list of items with `Description` and `Value` properties. Additionally, the `index` tag is used to create a CSS class with increasing numbers.
    
## The 'with' tag
Within a block of text, you may refer to a same top-level placeholder over and over. You can cut down the amount of text by using the **with** tag.

    {{#with Customer.Address}}
    {{FirstName}} {{LastName}}
    {{Line1}}
    {{#if Line2}}
    {{Line2}}
    {{/if}}
    {{#if Line3}}
    {{Line3}}
    {{/if}}
    {{City}} {{State}}, {{ZipCode}}
    {{/with}}
    
Here, the `Customer.Address` property will be searched first for the placeholders. If a property cannot be found in the `Address` object, it will be searched for in the `Customer` object and on up.

## The 'set' tag
**mustache#** provides limited support for variables through use of the `set` tag. Once a variable is declared, it is visible to all child scopes. Multiple definitions of a variable with the same name cannot be created within the same scope. In fact, I highly recommend making variable names unique to the entire template just to prevent unexpected behavior!

The following example will print out "EvenOddEvenOdd" by toggling a variable called `even`:

    FormatCompiler compiler = new FormatCompiler();
    const string format = @"{{#set even}}
    {{#each this}}
    {{#if @even}}
    Even
    {{#else}}
    Odd
    {{/if}}
    {{#set even}}
    {{/each}}";
    Generator generator = compiler.Compile(format);
    generator.ValueRequested += (sender, e) =>
    {
        e.Value = !(bool)(e.Value ?? false);
    };
    string result = generator.Render(new int[] { 0, 1, 2, 3 });
    
This code works by specifying a function to call whenever a value is needed for the `even` variable. The first time the function is called, `e.Value` will be null. All additional calls will hold the last known value of the variable.

Notice that when you set the variable, you don't qualify it with an `@`. You only need the `@` when you request its value, like in the `if` statement above.
    
You should attempt to limit your use of variables within templates. Instead, perform as many up-front calculations as possible and make sure your view model closely represents its final appearance. In this case, it would make more sense to first convert the array into strings of "Even" and "Odd".

    FormatCompiler compiler = new FormatCompiler();
    const string format = @"{{#each this}}{{this}}{{/each}}";
    Generator generator = compiler.Compile(format);
    string result = generator.Render(new string[] { "Even", "Odd", "Even", "Odd" });

This code is much easier to read and understand. It is also going to run significantly faster. In cases where you also need the original value, you can create an array containing objects with properties for the original value *and* `Even`/`Odd`.

## Defining Your Own Tags
If you need to define your own tags, **mustache#** has everything you need.

Once you define your own tags, you can register them with the compiler using the `RegisterTag` method.

    FormatCompiler compiler = new FormatCompiler();
    compiler.RegisterTag(myTag);
    
Your tag can be referenced within the template by leading its name with a `#`.

Custom tags can take any number of parameters. Parameters can have default values if you don't want to pass them all the time. Arguments are passed by specifying a placeholder.

### Multi-line Tags
Here's an example of a tag that will make all of its content upper case:

    public class UpperTagDefinition : ContentTagDefinition
    {
        public UpperTagDefinition()
            : base("upper")
        {
        }
        
        public override IEnumerable<NestedContext> GetChildContext(TextWriter writer, KeyScope scope, Dictionary<string, object> arguments)
        {
            NestedContext context = new NestedContext() 
            { 
                KeyScope = scope, 
                Writer = new StringWriter(), 
                WriterNeedsConsolidated = true,
            };
            yield return context;
        }
        
        public override string ConsolidateWriter(TextWriter writer, Dictionary<string, object> arguments)
        {
            return writer.ToString().ToUpperInvariant();
        }
    }
    
Another solution is to wrap the given TextWriter with another TextWriter that will change the case of the strings passed to it. This approach requires more work, but would be more efficient. You should attempt to wrap or reuse the text writer passed to the tag.
    
### In-line Tags
Here's an example of a tag that will join the items of a collection:

    public class JoinTagDefinition : InlineTagDefinition
    {
        public JoinTagDefinition()
            : base("join")
        {
        }
        
        protected override IEnumerable<TagParameter> GetParameters()
        {
            return new TagParameter[] { new TagParameter("collection") };
        }
        
        protected override void GetText(TextWriter writer, Dictionary<string, object> arguments)
        {
            IEnumerable collection = (IEnumerable)arguments["collection"];
            string joined = String.Join(", ", collection.Cast<object>().Select(o => o.ToString()));
            writer.Write(joined);
        }
    }

## License
If you are looking for a license, you won't find one. The software in this project is free, as in "free as air". Feel free to use my software anyway you like. Use it to build up your evil war machine, swindle old people out of their social security or crush the souls of the innocent.

I love to hear how people are using my code, so drop me a line. Feel free to contribute any enhancements or documentation you may come up with, but don't feel obligated. I just hope this code makes someone's life just a little bit easier.
