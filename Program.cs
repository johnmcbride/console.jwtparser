using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

var app = new CommandApp();

app.Configure(config => {
    config.SetApplicationName("jwtconsole");
    config.AddCommand<ExtractCommand>("extract")
    .WithExample("extract","eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")
    .WithExample("extract","eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c","--pretty")
    .WithExample("extract","eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c","-p");
});
app.Run(args);

public class ExtractCommand : Command<ExtractCommand.ExtractSettings>
{
    public class ExtractSettings : CommandSettings
    {
        [Description("The encoded JWT that needs to be decoded.")]
        [CommandArgument(0,"<JWT>")]
        public string JWT { get; set; }
        [Description("Decrypt the JWT (NOT IMPLEMENTED)")]
        [CommandOption("-d|--decrypt")]
        public bool? Decrypt { get; set; }

        [Description("Display the decoded JSON colorized and formatted")]
        [CommandOption("-p|--pretty")]
        public bool? PrettyPrint { get; set; }
    }
    public override int Execute(CommandContext context, ExtractSettings settings)
    {
        AnsiConsole.MarkupLine($"[yellow]Input JWT value[/]");
        AnsiConsole.MarkupLine($"[green]{settings.JWT}[/]\n");
        if ( settings.Decrypt == true )
        {
            //check for base 64 encoded
            // var isValidBase64 = Convert.TryFromBase64String(settings.JWT, out var j);
            var bytes = Convert.FromBase64String(settings.JWT);
        }

        //jwt decode
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = null;
        try
        {
            token = jwtTokenHandler.ReadJwtToken(settings.JWT);
        }
        catch (System.Exception)
        {
            AnsiConsole.MarkupLine($"[red]Unable to parse JWT (Invalid JWT)[/]");
            return 0;
        }
        
        //get the header and payload objects from the token
        var jwtHeader = FixInvalidCharacters(token.EncodedHeader);
        var jwtPayload = FixInvalidCharacters(token.EncodedPayload);
        var jwtSignature = FixInvalidCharacters(token.RawSignature);

        //decode base64 encoded header
        //check for valid b64
        if ( ! CheckForValidBase64(jwtHeader))
        {
            //invalid base 64
            AnsiConsole.MarkupLine($"[red]Invalid Base64 (HEADER). Trying to add padding[/]");
            //try and add padding to base64 string (==) length/divisible by 4
            jwtHeader = PadBase64String(jwtHeader);
            AnsiConsole.MarkupLine($"[yellow]Updated (PAYLOAD - padded).[/]");
        }
        var decodedHeaderBytes = Convert.FromBase64String(jwtHeader);
        string decodedHeader = System.Text.Encoding.UTF8.GetString(decodedHeaderBytes);

        //print token header info
        AnsiConsole.MarkupLine($"[yellow]JWT HEADER (Original Encoded)[/]");
        AnsiConsole.MarkupLine($"[white]{jwtHeader}[/]");
        
        AnsiConsole.MarkupLine($"[yellow]JWT HEADER (Decoded)[/]");
        if ( settings.PrettyPrint == true)
        { 
            var prettyHeaderJson = new JsonText(decodedHeader);
            AnsiConsole.Write(prettyHeaderJson);
            AnsiConsole.Write("\n\n");
        }
        else
        {
            AnsiConsole.MarkupLine($"[white]{decodedHeader}[/]\n");
        }

        AnsiConsole.MarkupLine($"[yellow]JWT PAYLOAD (Original Encoded)[/]");
        AnsiConsole.MarkupLine($"[white]{jwtPayload}[/]\n");
        //decode base64 encoded payload
        //check and fix the base64 encode
        if ( ! CheckForValidBase64(jwtPayload))
        {
            AnsiConsole.MarkupLine($"[red]Invalid Base64 (PAYLOAD). Trying to add padding[/]");
            //try and add padding to base64 string (==)
            jwtPayload = PadBase64String(jwtPayload);
            AnsiConsole.MarkupLine($"[yellow]Updated (PAYLOAD - padded).[/]");
            AnsiConsole.MarkupLine($"[orange1]{jwtPayload}[/]\n");
        }
        var decodedPayloadBytes = Convert.FromBase64String(jwtPayload);
        string decodePayload = System.Text.Encoding.UTF8.GetString(decodedPayloadBytes);

        //print payload info
        AnsiConsole.MarkupLine($"[yellow]JWT PAYLOAD (Decoded)[/]");
        if ( settings.PrettyPrint == true)
        {
            var prettyPayloadJson = new JsonText(decodePayload);
            AnsiConsole.Write(prettyPayloadJson);
        }
        else
        {
            AnsiConsole.MarkupLine($"[white]{decodePayload}[/]\n");
        }

        return 0;
    }
    public bool CheckForValidBase64(string Base64Item)
    {
        if ( Base64Item.Replace(" ","").Length % 4 == 0 )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public string FixInvalidCharacters(string Base64Item)
    {
        var b64 = Base64Item.Replace("_","/")
            .Replace("-","+");
        
        return b64;
    }
    public string PadBase64String(string Base64Item)
    {
        //get the remainder left from mod...
        var numberToAdd = Base64Item.Length % 4;
        //if remainder if greater that two, subtract two as we can
        //only add two padding characters
        numberToAdd = numberToAdd <= 2 ? numberToAdd : numberToAdd - 2;
        //add any padding characters
        Base64Item += new string('=',numberToAdd);

        return Base64Item;
    }
}

