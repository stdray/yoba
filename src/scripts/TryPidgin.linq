<Query Kind="Statements">
  <NuGetReference>Pidgin</NuGetReference>
  <Namespace>Pidgin</Namespace>
  <Namespace>static Pidgin.Parser</Namespace>
</Query>

var Colon = Char(':');
var ColonWhitespace = Colon.Between(SkipWhitespaces);
var Comma = Char(',');

var Bot = OneOf(String("yoba"), String("ёба")).Between(SkipWhitespaces);
var Sep = OneOf(String(","), String("или"));
var Text = LetterOrDigit.Or(Whitespace).AtLeastOnce();
var Question = Text.Between(String("вангуй"), String(":"));
var Answers = Text.Then(Sep.Then(Text).Many());
var End = String("?").Optional();
var Vanga = Bot.Then(Question).Then(Answers);
Vanga.Parse("ёба вангуй кодач енто: транскоалиция, чат пидоров или просто лелки").Dump();