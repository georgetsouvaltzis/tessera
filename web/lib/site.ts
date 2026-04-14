export const basePath = process.env.NEXT_PUBLIC_BASE_PATH?.trim() ?? "";
const configuredDocsOrigin = process.env.NEXT_PUBLIC_DOCS_ORIGIN?.trim() ?? "";
const localDocsOrigin = process.env.NODE_ENV === "development" ? "http://127.0.0.1:8000" : "";
export const docsOrigin = configuredDocsOrigin || localDocsOrigin;

export function appHref(path: string): string {
    if (path.length === 0 || path === "/") {
        return basePath || "/";
    }

    const normalized = path.startsWith("/") ? path : `/${path}`;
    return `${basePath}${normalized}`;
}

export function docsHref(path: string = "/"): string {
    const normalized = path === "/" ? "/" : path.startsWith("/") ? path : `/${path}`;
    if (docsOrigin.length > 0) {
        return `${docsOrigin}${normalized}`;
    }

    return `${basePath}/docs${normalized === "/" ? "/" : normalized}`;
}
