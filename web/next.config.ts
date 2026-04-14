import type { NextConfig } from "next";

const rawBasePath = process.env.PAGES_BASE_PATH?.trim() ?? "";
const basePath =
    rawBasePath.length === 0
        ? ""
        : rawBasePath.startsWith("/")
            ? rawBasePath
            : `/${rawBasePath}`;

const nextConfig: NextConfig = {
    output: "export",
    trailingSlash: true,
    images: {
        unoptimized: true,
    },
    basePath,
};

export default nextConfig;
