import React from 'react';
import { Link, useMatches, useLocation, RouteObject } from 'react-router-dom';
import { ChevronRight, GitBranch } from 'lucide-react';

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
    const [openDropdownIndex, setOpenDropdownIndex] = React.useState<number | null>(null);

    const findSiblingRoutes = (pathname: string, currentRoutes: ExtendedRouteObject[]): ExtendedRouteObject[] => {
        for (const route of currentRoutes) {
            if (route.path === pathname) {
                return [];
            }
            if (route.children) {
                const siblings = route.children.filter(r => r.path !== pathname && r.handle?.label);
                if (siblings.length > 0) {
                    return siblings;
                }
                const nestedSiblings = findSiblingRoutes(pathname, route.children);
                if (nestedSiblings.length > 0) {
                    return nestedSiblings;
                }
            }
        }
        return [];
    };

    const renderBreadcrumbItem = (match: UIMatch, index: number, array: UIMatch[]) => {
        const siblings = findSiblingRoutes(match.pathname, routes);
        const isLast = index === array.length - 1;

        return (
            <React.Fragment key={match.pathname}>
                <div className="flex items-center">
                    <Link
                        to={match.pathname}
                        className={`text-sm hover:text-blue-600 transition-colors ${
                            isLast ? 'text-blue-600 font-semibold' : 'text-gray-600'
                        }`}
                    >
                        {match.handle.label}
                    </Link>

                    {siblings.length > 0 && (
                        <div className="relative ml-2">
                            <button
                                onClick={() => setOpenDropdownIndex(openDropdownIndex === index ? null : index)}
                                className="group flex items-center"
                            >
                                <GitBranch
                                    className="w-4 h-4 text-gray-400 transform rotate-225 hover:text-blue-500 transition-colors"
                                />
                            </button>

                            {openDropdownIndex === index && (
                                <div className="absolute left-0 mt-2 w-48 bg-white rounded-md shadow-lg z-10 py-1">
                                    {siblings.map((route) => (
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