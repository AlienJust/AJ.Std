﻿using AJ.Std.Composition.Contracts;

namespace AJ.Std.Composition
{
	public class CompositionPartWithInformationRelay : ICompositionPartWithInformation {
		private readonly ICompositionPart _relayPart;

		public CompositionPartWithInformationRelay(ICompositionPart relayPart) {
			_relayPart = relayPart;
			IsInitComplete = false;
		}

		public void SetCompositionRoot(ICompositionRoot root) {
			_relayPart.SetCompositionRoot(root);
		}

		public bool IsInitComplete { get; set; }

		public ICompositionPart CompositionPart => _relayPart;
	}
}