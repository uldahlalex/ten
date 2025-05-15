import React from 'react';
import { Link, useMatches, useLocation } from 'react-router-dom';

interface RouteHandle {
    label: string;
}

interface UIMatch {
    id: string;
    pathname: string;
    params: Record<string, string>;
    data: unknown;
    handle: RouteHandle | undefined;
}

export const Breadcrumbs = () => {
    const matches = useMatches() as UIMatch[];

    // Filter out duplicate paths while keeping the most specific match
    const uniqueMatches = matches.reduce((acc, current) => {
        const existing = acc.find(m => m.pathname === current.pathname);
        if (existing) {
            // Replace the existing match if the current one has a handle
            if (current.handle && current.handle.label) {
                const index = acc.indexOf(existing);
                acc[index] = current;
            }
        } else {
            acc.push(current);
        }
        return acc;
    }, [] as UIMatch[]);

    return (
        <nav className="p-4 bg-white shadow-sm rounded-lg">
            <div className="flex items-center space-x-2">
                {uniqueMatches
                    .filter((match) => match.handle && match.handle.label)
                    .map((match, index, array) => (
                        <React.Fragment
                            key={`${match.pathname}-${match.handle?.label}`}
                        >
                            <Link
                                to={match.pathname}
                                className={`text-sm hover:text-blue-600 transition-colors ${
                                    index === array.length - 1
                                        ? 'text-blue-600 font-semibold'
                                        : 'text-gray-600'
                                }`}
                            >
                                {match.handle?.label}
                            </Link>
                            {index < array.length - 1 && (
                                <p> 👉 </p>
                            )}
                        </React.Fragment>
                    ))}
            </div>
        </nav>
    );
};

export default Breadcrumbs;