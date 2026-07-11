using System;
using System.Collections.Generic;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

[DocumentService]
public interface ITextMarkerService
{
	IEnumerable<ITextMarker> TextMarkers { get; }

	ITextMarker Create(int startOffset, int length);

	void Remove(ITextMarker marker);

	void RemoveAll(Predicate<ITextMarker> predicate);

	IEnumerable<ITextMarker> GetMarkersAtOffset(int offset);
}
