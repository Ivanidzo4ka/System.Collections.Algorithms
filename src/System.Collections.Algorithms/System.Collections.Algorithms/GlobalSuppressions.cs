// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "I don't like single line braces.", Scope = "namespaceanddescendants", Target = "System.Collections.Algorithms")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "I prefer underscore to this.", Scope = "namespaceanddescendants", Target = "System.Collections.Algorithms")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "I prefer underscore to this.", Scope = "namespaceanddescendants", Target = "System.Collections.Algorithms")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "I need to come up with header first.", Scope = "namespace", Target = "~N:System.Collections.Algorithms")]
