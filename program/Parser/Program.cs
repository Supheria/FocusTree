//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Parser;

List<TokenAPI> tokens = new();
Tokenizer parser = new("test.txt", tokens);
Console.Read();