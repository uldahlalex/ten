export const pathIncludes = (path: string, location): boolean => {
    //console.log("Comparing: "+path+ " TO: "+location.pathname)
    return location.pathname.includes(path);
};

export const pathIsExactly = (path: string, location): boolean => {
    console.log("Comparing: "+path+ " TO: "+location.pathname)
    return location.pathname === path;
};