using System.Reflection;

[assembly: Obfuscation(Feature = "type renaming pattern '__DARKED'.*", Exclude = false)]
[assembly: Obfuscation(Feature = "encrypt symbol names with password dtfsmFvOypXlTHjPv0bg4zZ8s7VZi3MImCnxqp5gfFP1LBburDkLVmsLjXuXex8L", Exclude = false)]
#pragma warning disable EF2708 // Issue with ObfuscationAttribute.Exclude property value.
[assembly: Obfuscation(Feature = "string encryption", Exclude = false)]
#pragma warning restore EF2708 // Issue with ObfuscationAttribute.Exclude property value.
[assembly: Obfuscation(Feature = "encrypt resources", Exclude = false)]
[assembly: Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
[assembly: Obfuscation(Feature = "rename symbol names with printable characters", Exclude = false)]