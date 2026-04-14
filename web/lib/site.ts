export const basePath = process.env.NEXT_PUBLIC_BASE_PATH?.trim() ?? "";
export const docsOrigin = process.env.NEXT_PUBLIC_DOCS_ORIGIN?.trim() ?? "";

export function appHref(path: string): string {
    if (path.length === 0 || path === "/") {
        return basePath || "/";
    }

    const normalized = path.startsWith("/") ? path : `/${path}`;
    return `${basePath}${normalized}`;
}

export function docsHref(path: string = "/"): string {
    const normalized = path === "/" ? "/" : path.startsWith("/") ? path : `/${path}`;
    const docsPath = `${basePath}/docs${normalized === "/" ? "/" : normalized}`;
    return docsOrigin.length > 0 ? `${docsOrigin}${docsPath}` : docsPath;
}
