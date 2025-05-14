import React from 'react';
import { Link, useMatches, useLocation, RouteObject } from 'react-router-dom';
import { ChevronRight, GitBranch, ChevronsDown } from 'lucide-react';

interface RouteHandle {
    label: string;
}

interface ExtendedRouteObject {
    caseSensitive?: boolean;
    children?: ExtendedRouteObject[];
    element?: React.ReactNode;
    index?: boolean;
    path?: string;
    handle?: RouteHandle;
    id?: string;
}

interface UIMatch {
    id: string;
    pathname: string;
    params: Record<string, string>;
    data: unknown;
    handle: RouteHandle | undefined;
}

interface RelatedRoutes {
    siblings: ExtendedRouteObject[];
    children: ExtendedRouteObject[];
}

export const Breadcrumbs = ({ routes }: { routes: ExtendedRouteObject[] }) => {
    const matches = useMatches() as UIMatch[];
    const [openDropdownIndex, setOpenDropdownIndex] = React.useState<number | null>(null);

    const findRelatedRoutes = (matches: UIMatch[], currentIndex: number, routes: ExtendedRouteObject[]): RelatedRoutes => {
        const currentPathSegments = matches[currentIndex].pathname.split('/').filter(Boolean);

        const traverseRoutes = (
            currentRoutes: ExtendedRouteObject[],
            depth: number
        ): RelatedRoutes => {
            if (depth >= currentPathSegments.length) {
                return { siblings: [], children: [] };
            }

            for (const route of currentRoutes) {
                const routePathSegments = (route.path || '').split('/').filter(Boolean);
                const currentSegment = currentPathSegments[depth];

                if (routePathSegments[routePathSegments.length - 1] === currentSegment) {
                    const siblings = currentRoutes
                        .filter(r =>
                            r !== route &&
                            r.handle?.label &&
                            r.path &&
                            !r.path.includes(':')
                        );

                    const children = (route.children || [])
                        .filter(r =>
                            r.handle?.label &&
                            r.path &&
                            !r.path.includes(':')
                        );

                    if (depth === currentPathSegments.length - 1) {
                        return { siblings, children };
                    }

                    if (route.children) {
                        return traverseRoutes(route.children, depth + 1);
                    }
                }
            }

            return { siblings: [], children: [] };
        };

        return traverseRoutes(routes, 0);
    };

    const renderBreadcrumbItem = (match: UIMatch, index: number, array: UIMatch[]) => {
        const { siblings, children } = findRelatedRoutes(matches, index, routes);
        const isLast = index === array.length - 1;
        const hasRelatedRoutes = siblings.length > 0 || children.length > 0;

        return (
            <React.Fragment key={match.pathname}>
                <div className="flex items-center">
                    <Link
                        to={match.pathname}
                        className={`text-sm hover:text-blue-600 transition-colors ${
                            isLast ? 'text-blue-600 font-semibold' : 'text-gray-600'
                        }`}
                    >
                        {match.handle?.label}
                    </Link>

                    {hasRelatedRoutes && (
                        <div className="relative ml-2">
                            <button
                                onClick={() => setOpenDropdownIndex(openDropdownIndex === index ? null : index)}
                                className="group flex items-center"
                                title="Show related pages"
                            >
                                {siblings.length > 0 ? (
                                    <GitBranch
                                        className="w-4 h-4 text-gray-400 transform rotate-225 hover:text-blue-500 transition-colors"
                                    />
                                ) : (
                                    <ChevronsDown
                                        className="w-4 h-4 text-gray-400 hover:text-blue-500 transition-colors"
                                    />
                                )}
                            </button>

                            {openDropdownIndex === index && (
                                <div className="absolute left-0 mt-2 w-48 bg-white rounded-md shadow-lg z-10 py-1">
                                    {siblings.length > 0 && (
                                        <>
                                            <div className="px-4 py-1 text-xs font-semibold text-gray-500 bg-gray-50">
                                                Related Pages
                                            </div>
                                            {siblings.map((route) => (
                                                <Link
                                                    key={route.path}
                                                    to={route.path || ''}
                                                    className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-blue-600 transition-colors"
                                                >
                                                    {route.handle?.label}
                                                </Link>
                                            ))}
                                        </>
                                    )}
                                    {children.length > 0 && (
                                        <>
                                            <div className="px-4 py-1 text-xs font-semibold text-gray-500 bg-gray-50">
                                                Sub Pages
                                            </div>
                                            {children.map((route) => (
                                                <Link
                                                    key={route.path}
                                                    to={route.path || ''}
                                                    className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-blue-600 transition-colors"
                                                >
                                                    {route.handle?.label}
                                                </Link>
                                            ))}
                                        </>
                                    )}
                                </div>
                            )}
                        </div>
                    )}
                </div>

                {!isLast && (
                    <ChevronRight className="w-4 h-4 mx-2 text-gray-400" />
                )}
            </React.Fragment>
        );
    };

    return (
        <nav className="p-4 bg-white shadow-sm rounded-lg">
            <div
                className="flex items-center flex-wrap gap-y-2"
                onClick={(e) => {
                    if (!(e.target as HTMLElement).closest('button')) {
                        setOpenDropdownIndex(null);
                    }
                }}
            >
                {matches
                    .filter((match): match is UIMatch => Boolean(match.handle?.label))
                    .map(renderBreadcrumbItem)}
            </div>
        </nav>
    );
};

export default Breadcrumbs;