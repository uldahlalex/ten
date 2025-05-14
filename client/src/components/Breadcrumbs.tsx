import { useMatches, Link } from 'react-router-dom';
import { ChevronRight } from 'lucide-react';
import { useState, useEffect } from 'react';

// Define the structure for route metadata
interface BreadcrumbMetadata {
    label?: string;
    hideInBreadcrumb?: boolean;
}

// Helper type for the route with handle
interface RouteWithHandle {
    id: string;
    pathname: string;
    params: Record<string, string>;
    data: unknown;
    handle?: BreadcrumbMetadata;
}

export const Breadcrumbs = () => {
    const matches = useMatches() as RouteWithHandle[];
    const [breadcrumbs, setBreadcrumbs] = useState<Array<{path: string, label: string}>>([]);

    useEffect(() => {
        // Filter out routes that should be hidden
        const visibleMatches = matches.filter(match => {
            // Skip routes with hideInBreadcrumb flag
            if (match.handle?.hideInBreadcrumb) {
                return false;
            }
            return true;
        });

        // Create breadcrumb items from visible matches
        const items = visibleMatches.map(match => {
            // Extract the label directly from the route's handle property
            // If no label is provided, use the last segment of the pathname
            const label = match.handle?.label ||
                (match.pathname === '/' ? 'Home' :
                    match.pathname.split('/').pop() || 'Home');

            return {
                path: match.pathname,
                label: label
            };
        });

        setBreadcrumbs(items);
    }, [matches]);

    // Don't show breadcrumbs if we're just at the root or there's only one item
    if (breadcrumbs.length <= 1) {
        return null;
    }

    return (
        <nav aria-label="Breadcrumb" className="px-4 py-2 bg-gray-50 rounded-md mb-4">
            <ol className="flex items-center space-x-1 text-sm">
                {breadcrumbs.map((crumb, index) => {
                    const isLast = index === breadcrumbs.length - 1;

                    return (
                        <li key={crumb.path} className="flex items-center">
                            {index > 0 && <ChevronRight className="h-4 w-4 text-gray-400 mx-1" />}

                            {isLast ? (
                                <span className="font-medium text-gray-700" aria-current="page">
                  {crumb.label}
                </span>
                            ) : (
                                <Link
                                    to={crumb.path}
                                    className="text-blue-600 hover:text-blue-800 hover:underline"
                                >
                                    {crumb.label}
                                </Link>
                            )}
                        </li>
                    );
                })}
            </ol>
        </nav>
    );
};