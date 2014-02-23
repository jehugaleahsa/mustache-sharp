using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mustache {
	/// <summary>
	/// Defines a tag that renders its content depending on the truthyness
	/// of its argument, with optional elif and else nested tags.
	/// </summary>
	internal sealed class UnlessTagDefinition : ConditionTagDefinition {
		/// <summary>
		/// Initializes a new instance of a IfTagDefinition.
		/// </summary>
		public UnlessTagDefinition()
			: base("unless") {
		}

		/// <summary>
		/// Gets whether the tag only exists within the scope of its parent.
		/// </summary>
		protected override bool GetIsContextSensitive() {
			return false;
		}

		/// <summary>
		/// Gets the tags that come into scope within the context of the current tag.
		/// </summary>
		/// <returns>The child tag definitions.</returns>
		protected override IEnumerable<string> GetChildTags() {
			return new string[0];
		}

		/// <summary>
		/// Gets whether the given tag's generator should be used for a secondary (or substitute) text block.
		/// </summary>
		/// <param name="definition">The tag to inspect.</param>
		/// <returns>True if the tag's generator should be used as a secondary generator.</returns>
		public override bool ShouldCreateSecondaryGroup(TagDefinition definition) {
			return false;
		}

		/// <summary>
		/// Gets whether the primary generator group should be used to render the tag.
		/// </summary>
		/// <param name="arguments">The arguments passed to the tag.</param>
		/// <returns>
		/// True if the primary generator group should be used to render the tag;
		/// otherwise, false to use the secondary group.
		/// </returns>
		public override bool ShouldGeneratePrimaryGroup(Dictionary<string, object> arguments) {
			return !base.ShouldGeneratePrimaryGroup(arguments);
		}

	}
}
