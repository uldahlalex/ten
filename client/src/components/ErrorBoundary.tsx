import React from 'react';

class EnhancedErrorBoundary extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            hasError: false,
            error: null,
            errorInfo: null
        };
    }

    static getDerivedStateFromError(error) {
        return { hasError: true };
    }

    componentDidCatch(error, errorInfo) {
        // Capture the component stack
        this.setState({
            error: error,
            errorInfo: errorInfo
        });

        // Log to console for better stack trace
        console.group('🔴 Error Boundary Caught Error');
        console.error('Error:', error);
        console.error('Component Stack:', errorInfo.componentStack);

        // Create a clean component stack trace
        const componentStack = errorInfo.componentStack
            .split('\n')
            .map(line => line.trim())
            .filter(line => line.startsWith('at'))
            .map(line => {
                // Extract component name and file location
                const match = line.match(/at (.*) \((.*)\)/);
                if (match) {
                    return {
                        component: match[1],
                        location: match[2]
                    };
                }
                return { component: line, location: 'unknown' };
            });

        console.table(componentStack);
        console.groupEnd();

        // You could also send this to your error tracking service
        // e.g., Sentry, LogRocket, etc.
    }

    render() {
        if (this.state.hasError) {
            return (
                <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
                    <div className="max-w-2xl w-full bg-white rounded-lg shadow-lg p-6">
                        <div className="flex items-center space-x-2 text-red-600 mb-4">
                            <svg
                                className="w-6 h-6"
                                fill="none"
                                stroke="currentColor"
                                viewBox="0 0 24 24"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth={2}
                                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
                                />
                            </svg>
                            <h2 className="text-xl font-semibold">Something went wrong</h2>
                        </div>

                        <div className="bg-gray-100 rounded p-4 mb-4">
                            <p className="font-mono text-sm text-gray-800">
                                {this.state.error && this.state.error.toString()}
                            </p>
                        </div>

                        <div className="bg-gray-100 rounded p-4 overflow-auto max-h-64">
                            <pre className="font-mono text-xs text-gray-700 whitespace-pre-wrap">
                                {this.state.errorInfo && this.state.errorInfo.componentStack}
                            </pre>
                        </div>

                        <div className="mt-4 flex justify-end space-x-2">
                            <button
                                className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 transition-colors"
                                onClick={() => window.location.reload()}
                            >
                                Reload Page
                            </button>
                            <button
                                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
                                onClick={() => this.setState({ hasError: false })}
                            >
                                Try Again
                            </button>
                        </div>

                        {process.env.NODE_ENV === 'development' && (
                            <div className="mt-4 text-sm text-gray-500">
                                <p>👉 Check the console for more detailed error information and component stack trace.</p>
                            </div>
                        )}
                    </div>
                </div>
            );
        }

        return this.props.children;
    }
}

export default EnhancedErrorBoundary;