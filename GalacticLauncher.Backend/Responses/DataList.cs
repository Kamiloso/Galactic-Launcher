using System.Collections.Generic;

namespace GalacticLauncher.Backend.Responses;

public record DataList<T>(IEnumerable<T> Items);