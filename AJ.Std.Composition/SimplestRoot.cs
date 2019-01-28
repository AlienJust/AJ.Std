﻿using System;
using System.Collections.Generic;
using System.Linq;
using AJ.Std.Composition.Contracts;

namespace AJ.Std.Composition {
	class SimplestRoot : ICompositionRoot, ICompositionPartsRegPoint {
		private readonly List<ICompositionPart> _parts;

		public ICompositionPart GetPartByName(string partName) {
		 return _parts.First(p => p.Name == partName);
		}

		public void RegisterPart(ICompositionPart part) {
			throw new NotImplementedException();
		}
	}
}