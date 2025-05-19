using System;
using System.Collections.Generic;
using JetBrains.Application.UI.Utils;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.Misc;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.ProvidersAPI;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.Text;

namespace ReSharper.MediatorPlugin.Actions;

//  https://www.jetbrains.com/help/resharper/sdk/Navigation.html?_gl=1%2A1qta1kj%2A_gcl_au%2AMTQ2MDcwNjgwMS4xNzQ3NTgxNjk2%2AFPAU%2AMTQ2MDcwNjgwMS4xNzQ3NTgxNjk2%2A_ga%2ANDAxMzM1MjAuMTczODA4NTc4Nw..%2A_ga_9J976DJZ68%2AczE3NDc2NzM4NzgkbzIzJGcxJHQxNzQ3Njc2NDA1JGo1NSRsMCRoMCRkOXRuZTZJVHBvQkNRYkVla0duRldlazA0S3Q1RUM1am5NQQ..&_cl=MTsxOzE7VkJicGdyblBMZ0R3dmd4bkNESWRPMXJCT0s5WnJzazRaUDRteVRWaDk4WHExc2tFSTZ1NmxEUHhQZ25EbWxRQjs%3D#occurrence-presenter

public sealed class MediatorOccurrenceNavigationProvider : IOccurrenceNavigationProvider
{
    public bool IsApplicable
    (
        INavigationScope scope,
        GotoContext gotoContext,
        IIdentifierMatcher matcher
    )
    {
        throw new NotImplementedException();
    }

    public IEnumerable<MatchingInfo> FindMatchingInfos
    (
        IIdentifierMatcher matcher,
        INavigationScope scope,
        GotoContext gotoContext
    )
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IOccurrence> GetOccurrencesByMatchingInfo
    (
        MatchingInfo navigationInfo,
        INavigationScope scope,
        GotoContext gotoContext)
    {
        throw new NotImplementedException();
    }
}