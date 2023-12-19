// See https://aka.ms/new-console-template for more information

using static System.Runtime.InteropServices.JavaScript.JSType;

string newline = "\r\n";
string file = @"C:\Users\SaLiVa\source\repos\AdventOfCode2023Day3\Day3Input.txt";

List<PositionCharacter> engineSchematic = [];

using (StreamReader reader = File.OpenText(file))
{
    int xPos = 0, yPos = 0;
    while(reader.Peek() >= 0)
    {
        char c = (char)reader.Read();

        if (!newline.Contains(c))
        {
            engineSchematic.Add(new PositionCharacter(xPos, yPos, c));
            xPos += 1;
        }
        else
        {
            if(c.Equals('\n'))
                yPos += 1;
            xPos = 0;
        }
    }
}

HashSet<PositionString> digitList = [];

foreach (PositionCharacter character in engineSchematic)
{
    // Check if character is a symbol
    if (character.IsSymbol)
    {
        // Extract adjacent cells where cells contain a number
        var adjacentCells = engineSchematic.Where(x => x.X >= character.X - 1 && x.X <= character.X + 1 && x.Y <= character.Y + 1 && x.Y >= character.Y - 1 && x.IsNumber).ToList();
        
        if(character.IsGear)
        {
            character.GearIsEngaged = true;
        }

        foreach (var cell in adjacentCells)
        {
            int rightDigitLimit = 0;
            int leftDigitLimit = 0;

            string symbolPattern = @"[!@#$%^&*()_+=\[{\]};.:<>|/?,\\'""-]";
            
            // Find the right and left limits of the number
            // Left limit
            for (int i = cell.X; i >= 0; i--)
            {
                var testchar = engineSchematic.First(x => x.X == i && x.Y == cell.Y).C;
                if (char.IsNumber(testchar))
                { leftDigitLimit = i; continue; }
                if (symbolPattern.Contains(testchar))
                {
                    break; 
                }
            }
            
            // Right limit
            for (int i = cell.X; i < engineSchematic.Where(y => y.Y == cell.Y).Max(x => x.X) + 1; i++)
            {
                var testchar = engineSchematic.First(x => x.X == i && x.Y == cell.Y).C;
                if (char.IsNumber(testchar))
                { 
                    // If number is found
                    rightDigitLimit = i + 1;
                }
                if (symbolPattern.Contains(testchar))
                {
                    // If symbol is found - Make the right digit limit be the symbol position
                    rightDigitLimit = i;
                    break;
                }
            }

            string numstring = string.Empty;
            int number = 0;
            // Build the number string and parse it
            for(int i = leftDigitLimit; i < rightDigitLimit; i++) { numstring += engineSchematic.First(x => x.X == i && x.Y == cell.Y).C; }
            try
            {
                number = Int32.Parse(numstring);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var digit = new PositionString(leftDigitLimit, cell.Y, number);
            if(character.GearIsEngaged)
            {
                digit.Gear = character;
            }
            digitList.Add(digit);
        }
    }
}

int total = 0;
foreach(var c in engineSchematic)
{
    if(c.GearIsEngaged)
    {
        // Calculate the product of numbers around the Gear
        var gears = digitList.Where(d => d.Gear == c);
        if(gears.Count() == 2)
        { 
            var ch1 = gears.First().Number;
            var ch2 = gears.Last().Number;
            c.GearProduct = gears.First().Number * gears.Last().Number;
            total += c.GearProduct;
        }
    }
}

Console.WriteLine($"Done: " + digitList.Sum(item => item.Number));
Console.WriteLine("Geared Total: " + total);

public class PositionCharacter
{
    public PositionCharacter(int x, int y, char c) 
    { 
        X = x; Y = y; C = c; GearIsEngaged = false; GearProduct = 0;
    }
    public int X { get; set; }
    public int Y { get; set; } 
    public char C { get; set; }
    public bool IsSymbol 
    { 
        get 
        {
            string symbolPattern = @"[!@#$%^&*()_+=\[{\]};:<>|/?,\\'""-]"; 
            return symbolPattern.Contains(C);
        }
    }
    public bool IsGear
    {
        get
        {
            string gearPattern = @"*";
            return gearPattern.Contains(C);
        }
    }
    
    public bool GearIsEngaged { get; set; }
    public int GearProduct { get; set; }
    public bool IsNumber { get { return char.IsNumber(C); } }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not PositionCharacter) return false;
        else
        {
            return (X == ((PositionCharacter)obj).X && Y == ((PositionCharacter)obj).Y && C == ((PositionCharacter)obj).C);
        }
    }

}

public class PositionString
{
    public PositionString(int x, int y, int number)
    {
        X = x; Y = y; Number = number; 
    }

    // Starting position of the string
    public int X { get; set; }
    public int Y { get; set; }

    public PositionCharacter? Gear { get; set; }

    public int Number { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not PositionString) return false;
        else
        {
            return (X == ((PositionString) obj).X && Y == ((PositionString) obj).Y && Number == ((PositionString) obj).Number && Gear == ((PositionString) obj).Gear);
        }
    }
}



