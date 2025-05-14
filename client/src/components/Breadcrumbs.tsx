import React from 'react';
import { Link, useMatches, useLocation, RouteObject } from 'react-router-dom';
import { ChevronRight, ChevronsUpDown } from 'lucide-react';

interface RouteHandle {
    label: string;
}

interface ExtendedRouteObject extends RouteObject {
    handle?: RouteHandle;
    children?: ExtendedRouteObject[];
}

interface UIMatch {
    id: string;
    pathname: string;
    params: Record<string, string>;
    data: unknown;
    handle: RouteHandle | undefined;
}

export const Breadcrumbs = ({ routes }: { routes: ExtendedRouteObject[] }) => {
    const matches = useMatches() as UIMatch[];
    const location = useLocation();
    const [showSiblings, setShowSiblings] = React.useState(false);

    // Function to find sibling routes
    const findSiblingRoutes = (currentMatch: UIMatch): ExtendedRouteObject[] => {
        const parentMatches = matches.slice(0, -1);
        const lastParentMatch = parentMatches[parentMatches.length - 1];

        if (!lastParentMatch?.handle) return [];

        // Find the parent route object from your routes configuration
        const findParentRoute = (routes: ExtendedRouteObject[], path: string): ExtendedRouteObject | null => {
            for (const route of routes) {
                if (route.path === path) return route;
                if (route.children) {
                    const found = findParentRoute(route.children, path);
                    if (found) return found;
                }
            }
            return null;
        };

        // Get sibling routes from the parent's children
        const parentRoute = findParentRoute(routes, lastParentMatch.pathname);
        return (parentRoute?.children?.filter(route => route.path !== currentMatch.pathname) || []) as ExtendedRouteObject[];
    };

    const currentMatch = matches[matches.length - 1];
    const siblingRoutes = findSiblingRoutes(currentMatch);

    return (
        <nav className="p-4 bg-white shadow-sm rounded-lg">
            <div className="flex items-center justify-between">
                <div className="flex items-center space-x-2">
                    {matches
                        .filter((match): match is UIMatch => Boolean(match.handle?.label))
                        .map((match, index, array) => (
                            <React.Fragment key={match.pathname}>
                                <Link
                                    to={match.pathname}
                                    className={`text-sm hover:text-blue-600 transition-colors ${
                                        index === array.length - 1
                                            ? 'text-blue-600 font-semibold'
                                            : 'text-gray-600'
                                    }`}
                                >
                                    {match.handle.label}
                                </Link>
                                {index < array.length - 1 && (
                                    <ChevronRight className="w-4 h-4 text-gray-400" />
                                )}
                            </React.Fragment>
                        ))}
                </div>

                {siblingRoutes.length > 0 && (
                    <div className="relative">
                        <button
                            onClick={() => setShowSiblings(!showSiblings)}
                            className="flex items-center space-x-1 text-sm text-gray-600 hover:text-blue-600 transition-colors"
                        >
                            <span>Related Pages</span>
                            <ChevronsUpDown className="w-4 h-4" />
                        </button>

                        {showSiblings && (
                            <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg z-10 py-1">
                                {siblingRoutes.map((route) => (
                                    <Link
                                        key={route.path}
                                        to={route.path || ''}
                                        className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-blue-600 transition-colors"
                                    >
                                        {route.handle?.label}
                                    </Link>
                                ))}
                            </div>
                        )}
                    </div>
                )}
            </div>
        </nav>
    );
};

export default Breadcrumbs;