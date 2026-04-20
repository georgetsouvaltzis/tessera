#!/usr/bin/env node

import fs from "node:fs";
import path from "node:path";

const repoRoot = process.cwd();
const controlsDir = path.join(repoRoot, "src", "Tessera", "Controls");
const outputDir = path.join(repoRoot, "docs", "widgets");

const ignoredControlNames = new Set([
  "Control",
  "NotificationInbox",
]);

const familyMap = new Map([
  ["Accordion", "Inputs & Forms"],
  ["ActivityFeed", "Data & Inspection"],
  ["AutocompleteInput", "Inputs & Forms"],
  ["Button", "Inputs & Forms"],
  ["Choice", "Inputs & Forms"],
  ["ComboBox", "Inputs & Forms"],
  ["DataForm", "Inputs & Forms"],
  ["DatePicker", "Inputs & Forms"],
  ["FieldSet", "Inputs & Forms"],
  ["Form", "Inputs & Forms"],
  ["MultiSelect", "Inputs & Forms"],
  ["NumberInput", "Inputs & Forms"],
  ["RadioGroup", "Inputs & Forms"],
  ["Slider", "Inputs & Forms"],
  ["Stepper", "Inputs & Forms"],
  ["TagInput", "Inputs & Forms"],
  ["TextArea", "Inputs & Forms"],
  ["TextInput", "Inputs & Forms"],
  ["TimePicker", "Inputs & Forms"],
  ["Toggle", "Inputs & Forms"],
  ["ValidationSummary", "Inputs & Forms"],
  ["Wizard", "Inputs & Forms"],

  ["Breadcrumb", "Navigation & Workflow"],
  ["CommandBar", "Navigation & Workflow"],
  ["CommandPalette", "Navigation & Workflow"],
  ["FileExplorer", "Navigation & Workflow"],
  ["FuzzyFinder", "Navigation & Workflow"],
  ["GroupedListView", "Navigation & Workflow"],
  ["JumpList", "Navigation & Workflow"],
  ["KanbanBoard", "Navigation & Workflow"],
  ["ListView", "Navigation & Workflow"],
  ["MenuBar", "Navigation & Workflow"],
  ["Paginator", "Navigation & Workflow"],
  ["QuickOpenOverlay", "Navigation & Workflow"],
  ["SearchBox", "Navigation & Workflow"],
  ["SearchResultsView", "Navigation & Workflow"],
  ["SideNavRail", "Navigation & Workflow"],
  ["Tabs", "Navigation & Workflow"],
  ["Toolbar", "Navigation & Workflow"],
  ["TreeView", "Navigation & Workflow"],
  ["VirtualizedListView", "Navigation & Workflow"],

  ["DataGrid", "Data & Inspection"],
  ["CommandOutput", "Data & Inspection"],
  ["DiffView", "Data & Inspection"],
  ["InspectorPanel", "Data & Inspection"],
  ["JsonTreeView", "Data & Inspection"],
  ["KeyValueList", "Data & Inspection"],
  ["LogTailPanel", "Data & Inspection"],
  ["LogView", "Data & Inspection"],
  ["MarkdownView", "Data & Inspection"],
  ["PivotTable", "Data & Inspection"],
  ["ProcessListView", "Data & Inspection"],
  ["PropertyGrid", "Data & Inspection"],
  ["QueryBuilder", "Data & Inspection"],
  ["RichTextView", "Data & Inspection"],
  ["Table", "Data & Inspection"],
  ["TaskRunnerPanel", "Data & Inspection"],
  ["TerminalPanel", "Data & Inspection"],
  ["Timeline", "Data & Inspection"],
  ["TraceViewer", "Data & Inspection"],
  ["TreeTable", "Data & Inspection"],

  ["AreaPlot", "Dashboards & Plots"],
  ["Badge", "Dashboards & Plots"],
  ["BarChart", "Dashboards & Plots"],
  ["BoxPlot", "Dashboards & Plots"],
  ["BulletChart", "Dashboards & Plots"],
  ["CalendarMonthView", "Dashboards & Plots"],
  ["DashboardGrid", "Dashboards & Plots"],
  ["Gauge", "Dashboards & Plots"],
  ["HealthBoard", "Dashboards & Plots"],
  ["Heatmap", "Dashboards & Plots"],
  ["Histogram", "Dashboards & Plots"],
  ["LineChart", "Dashboards & Plots"],
  ["LinePlot", "Dashboards & Plots"],
  ["MiniLog", "Dashboards & Plots"],
  ["PlotPanel", "Dashboards & Plots"],
  ["ProgressBar", "Dashboards & Plots"],
  ["ScatterPlot", "Dashboards & Plots"],
  ["SchedulerTimeline", "Dashboards & Plots"],
  ["Sparkline", "Dashboards & Plots"],
  ["StatsCard", "Dashboards & Plots"],
  ["TelemetryChart", "Dashboards & Plots"],
  ["TreeMapChart", "Dashboards & Plots"],

  ["ContextMenu", "Shells & Overlays"],
  ["Dialog", "Shells & Overlays"],
  ["DockWorkspace", "Shells & Overlays"],
  ["EmptyState", "Shells & Overlays"],
  ["KeyBindingHelpDialog", "Shells & Overlays"],
  ["Label", "Shells & Overlays"],
  ["Modal", "Shells & Overlays"],
  ["Notifications", "Shells & Overlays"],
  ["PaneTabs", "Shells & Overlays"],
  ["PaletteEditor", "Shells & Overlays"],
  ["ResizablePaneGroup", "Shells & Overlays"],
  ["Spinner", "Shells & Overlays"],
  ["SplitView", "Shells & Overlays"],
  ["StatusBar", "Shells & Overlays"],
  ["ToastCenter", "Shells & Overlays"],
  ["TokenEditor", "Shells & Overlays"],
]);

const featuredWidgetDocs = new Map([
  [
    "Button",
    {
      whenToUse: [
        "A primary action should be explicit and keyboard/pointer reachable.",
        "You need a low-friction call-to-action in a form or shell footer.",
      ],
      gotchas: [
        "Avoid using Button for passive labels; use `Label` or `Badge` instead.",
        "Do not hide business state changes in render-only code paths.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var refresh = new Button { Text = "Refresh orders" };
var status = new StatusBar { LeftText = "Ready" };
var count = 12;

refresh.Activated += (_, _) =>
{
    count++;
    status.LeftText = $"Orders: {count}";
};

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(refresh, width: 24, height: 3));
});`,
    },
  ],
  [
    "TextInput",
    {
      whenToUse: [
        "Single-line text entry (search, names, ids, filters).",
        "You need submit/cancel semantics for an inline editor.",
      ],
      gotchas: [
        "Use `TextArea` for multi-line text; avoid forcing line breaks into `TextInput`.",
        "Treat submitted text as a message into app state, not as implicit side effect.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var query = new TextInput
{
    Title = "Search",
    PlaceholderText = "order id, owner, region"
};
var status = new StatusBar { LeftText = "Type and press Enter" };

query.Submitted += (_, e) => status.LeftText = $"Applied: {e.Text}";
query.Cancelled += (_, _) => status.LeftText = "Search cleared";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(query, width: 56, height: 3));
});`,
    },
  ],
  [
    "NumberInput",
    {
      whenToUse: [
        "Bounded numeric values (retries, page size, thresholds).",
        "You need direct numeric editing without parsing text yourself.",
      ],
      gotchas: [
        "Always set domain-valid min/max limits.",
        "Avoid text parsing in `Update(...)` if `NumberInput` already enforces numeric shape.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var pageSize = new NumberInput
{
    Title = "Page size",
    MinValue = 10,
    MaxValue = 500,
    Value = 100,
    Step = 10
};

var status = new StatusBar { LeftText = "Adjust page size" };
pageSize.Submitted += (_, e) => status.LeftText = $"Page size: {e.Value}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(pageSize, width: 42, height: 3));
});`,
    },
  ],
  [
    "Choice",
    {
      whenToUse: [
        "One-of-many selection with a short option list.",
        "You want explicit selection state in a compact footprint.",
      ],
      gotchas: [
        "Prefer `ComboBox` when users need to type and narrow options.",
        "Keep option labels stable to avoid confusing selection persistence.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var lane = new Choice
{
    Title = "Lane",
    Items = { "Citrine", "Cobalt", "Ember" }
};

var status = new StatusBar { LeftText = "Select lane" };
lane.SelectionChanged += (_, _) => status.LeftText = $"Lane: {lane.SelectedItem}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(lane, width: 36, height: 7));
});`,
    },
  ],
  [
    "Form",
    {
      whenToUse: [
        "You want explicit rows with labels and controls.",
        "Validation/readability matter more than dense freeform layout.",
      ],
      gotchas: [
        "Use `DataForm<TModel>` when you need model-bound field registration.",
        "Avoid deeply nested forms; split into sections with `FieldSet`.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var name = new TextInput { PlaceholderText = "Order name" };
var qty = new NumberInput { Value = 1, MinValue = 1, MaxValue = 999 };
var submit = new Button { Text = "Create order" };

var form = new Form
{
    Title = "Create order",
    Fields =
    {
        FormField.Row("Name", name),
        FormField.Row("Quantity", qty),
        FormField.Row(string.Empty, submit)
    }
};

return Screen.Build(window => window.Body(body => body.Center(form, width: 56, height: 13)));`,
    },
  ],
  [
    "DataGrid",
    {
      whenToUse: [
        "You need record-heavy inspection with selection and sortable columns.",
        "The UI must handle dense tabular workflows.",
      ],
      gotchas: [
        "Keep column count intentional; avoid unreadable ultra-wide grids.",
        "Push expensive data refresh to effects/background workflows, not per-frame render.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var grid = new DataGrid
{
    Title = "Orders",
    ShowHeader = true,
    PageSize = 50
};

grid.SetColumns(
    new DataGridColumn("Id", 10),
    new DataGridColumn("Owner", 18),
    new DataGridColumn("Status", 12));

grid.SetRows(new[]
{
    new[] { "risk_10443", "luca ramos", "escalated" },
    new[] { "risk_10448", "nina maric", "watch" }
});

return Screen.Build(window => window.Body(body => body.Fill(grid)));`,
    },
  ],
  [
    "Tabs",
    {
      whenToUse: [
        "Users switch between a few stable views in one region.",
        "You need quick mode changes without route/context loss.",
      ],
      gotchas: [
        "Keep tab count small; move large navigation trees to `SideNavRail`.",
        "Preserve state per tab where possible to avoid user frustration.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var tabs = new Tabs();
tabs.SetItems("Workspace", "Inspect", "Actions");
tabs.SetSelectedIndex(0);

var content = new Label { Text = "Workspace view" };
tabs.SelectionChanged += (_, _) => content.Text = $"{tabs.SelectedItem} view";

return Screen.Build(window =>
{
    window.Body(body =>
    {
        body.Row(0.14f, tabs);
        body.Row(0.86f, content);
    });
});`,
    },
  ],
  [
    "ListView",
    {
      whenToUse: [
        "Straightforward item browsing with low complexity.",
        "You need selection-driven detail panels.",
      ],
      gotchas: [
        "Use `VirtualizedListView<T>` for very large collections.",
        "Do not overload each row with excessive formatting noise.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var list = new ListView<string>();
list.SetItems("Orders", "Incidents", "Telemetry", "Exports");

var status = new StatusBar { LeftText = "Select a section" };
list.SelectionChanged += (_, e) => status.LeftText = $"Selected: {e.SelectedItem}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Fill(list));
});`,
    },
  ],
  [
    "SideNavRail",
    {
      whenToUse: [
        "Primary shell navigation lives in a left rail.",
        "You need icon/badge selection with explicit activation.",
      ],
      gotchas: [
        "Use concise labels; long labels reduce scanability.",
        "Keep selected and activated behaviors consistent.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var rail = new SideNavRail();
rail.SetItems(
    new NavItem("workspace", "Workspace"),
    new NavItem("inspect", "Inspect"),
    new NavItem("actions", "Actions"));

var content = new Label { Text = "Workspace" };
rail.Activated += (_, e) => content.Text = $"Opened: {e.Item.Label}";

return Screen.Build(window =>
{
    window.Body(body =>
    {
        body.Row(0.22f, rail);
        body.Row(0.78f, content);
    });
});`,
    },
  ],
  [
    "CommandPalette",
    {
      whenToUse: [
        "Global command launch from keyboard-first workflows.",
        "Power users need fast action search and execution.",
      ],
      gotchas: [
        "Keep command names action-first (`Open`, `Run`, `Export`).",
        "Do not overload palette with low-value commands; keep it task-centric.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var palette = new CommandPalette
{
    Title = "Command Palette",
    IsOpen = true
};

palette.SetItems(
    new CommandPaletteItem("open-workspace", "Open workspace"),
    new CommandPaletteItem("export-csv", "Export CSV"),
    new CommandPaletteItem("restart-feed", "Restart feed"));

var status = new StatusBar { LeftText = "Type to search commands" };
palette.ItemExecuted += (_, e) => status.LeftText = $"Executed: {e.Item.Label}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(palette, width: 72, height: 16));
});`,
    },
  ],
  [
    "SearchBox",
    {
      whenToUse: [
        "Inline search with explicit next/prev match navigation.",
        "You need query state as a first-class part of the shell.",
      ],
      gotchas: [
        "Debounce expensive searches; avoid blocking UI loop per keystroke.",
        "Show match count and current index so navigation is predictable.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var search = new SearchBox
{
    Title = "Search slice",
    Placeholder = "entity, owner, region",
    MatchCount = 3,
    CurrentMatch = 1
};

var status = new StatusBar { LeftText = "Search ready" };
search.QueryChanged += (_, e) => status.LeftText = $"Query: {e.Query}";
search.NavigationRequested += (_, e) => status.LeftText = $"Navigate: {e.Direction}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(search, width: 68, height: 4));
});`,
    },
  ],
  [
    "Dialog",
    {
      whenToUse: [
        "User must confirm/cancel a disruptive action.",
        "You need a focused transient decision surface.",
      ],
      gotchas: [
        "Do not stack many dialogs; resolve one decision at a time.",
        "Always provide an explicit cancel path.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var dialog = new Dialog
{
    Title = "Delete draft?",
    Message = "This action cannot be undone.",
    IsOpen = true
};

var status = new StatusBar { LeftText = "Awaiting decision" };
dialog.Closed += (_, e) => status.LeftText = $"Dialog result: {e.Result}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(dialog, width: 54, height: 10));
});`,
    },
  ],
  [
    "StatusBar",
    {
      whenToUse: [
        "Persistent shell hints, shortcuts, and state summary.",
        "You need low-noise feedback without modal interruptions.",
      ],
      gotchas: [
        "Keep copy short; status bars are for signals, not paragraphs.",
        "Avoid using status bar as a replacement for validation surfaces.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var status = new StatusBar
{
    LeftText = "Orders: 127",
    RightText = "Enter refreshes   Ctrl+Q quits"
};

var body = new Label { Text = "Workspace shell" };

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(content => content.Fill(body));
});`,
    },
  ],
  [
    "SplitView",
    {
      whenToUse: [
        "First multi-pane layout before advanced docking.",
        "You need a stable two-region composition.",
      ],
      gotchas: [
        "Switch to `ResizablePaneGroup` when users need interactive pane resizing.",
        "Avoid deeply nested split trees; readability drops quickly.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var navigator = new ListView<string>();
navigator.SetItems("Orders", "Incidents", "Exports");

var detail = new MarkdownView { Markdown = "## Detail\\nSelect an item." };

var split = new SplitView
{
    Orientation = SplitViewOrientation.Horizontal,
    First = navigator,
    Second = detail,
    Ratio = 0.32f
};

return Screen.Build(window => window.Body(body => body.Fill(split)));`,
    },
  ],
  [
    "InspectorPanel",
    {
      whenToUse: [
        "Entity details need a dedicated structured side panel.",
        "You need key/value inspection with section grouping.",
      ],
      gotchas: [
        "Do not duplicate full row content from grid; surface only decision-relevant fields.",
        "Keep section ordering stable to build operator muscle memory.",
      ],
      usage: `using Tessera.Controls;
using Tessera.Layout;

var inspector = new InspectorPanel
{
    Title = "Record profile"
};

inspector.SetSections(
    new InspectorSection("Entity", new[]
    {
        new InspectorField("Id", "risk_10443"),
        new InspectorField("Owner", "luca ramos"),
        new InspectorField("Region", "eu-central-1")
    }));

return Screen.Build(window => window.Body(body => body.Fill(inspector)));`,
    },
  ],
]);

function readFilesRecursive(dir) {
  const output = [];
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      if (entry.name === "Internal") {
        continue;
      }
      output.push(...readFilesRecursive(full));
      continue;
    }
    if (!entry.name.endsWith(".cs")) {
      continue;
    }
    output.push(full);
  }
  return output;
}

function findControlClasses(files) {
  const controls = new Map();
  const pattern =
    /public\s+(?:sealed\s+)?(?:partial\s+)?class\s+([A-Za-z0-9_]+)(?:<[^>]+>)?\s*:\s*Control\b/gm;

  for (const file of files) {
    const text = fs.readFileSync(file, "utf8");
    let match = pattern.exec(text);
    while (match) {
      const className = match[1];
      if (!ignoredControlNames.has(className)) {
        controls.set(className, { className, files: new Set([file]) });
      }
      match = pattern.exec(text);
    }
  }

  for (const control of controls.values()) {
    for (const file of files) {
      const base = path.basename(file, ".cs");
      if (base === control.className || base.startsWith(`${control.className}.`)) {
        control.files.add(file);
      }
    }
  }

  return [...controls.values()].sort((a, b) => a.className.localeCompare(b.className));
}

function extractPublicMembers(control) {
  const properties = new Map();
  const events = new Map();

  const propertyPattern =
    /public\s+(?:override\s+)?(?:new\s+)?([A-Za-z0-9_<>\[\],?.\s:]+?)\s+([A-Za-z_][A-Za-z0-9_]*)\s*\{\s*get\s*;/gm;
  const eventPattern =
    /public\s+event\s+([A-Za-z0-9_<>\[\],?.\s:]+?)\s+([A-Za-z_][A-Za-z0-9_]*)\s*;/gm;

  for (const file of control.files) {
    const text = fs.readFileSync(file, "utf8");
    let propMatch = propertyPattern.exec(text);
    while (propMatch) {
      const type = propMatch[1].replace(/\s+/g, " ").trim();
      const name = propMatch[2].trim();
      if (
        !["Width", "Height", "IsFocusable", "Bounds", "Parent", "DebugName"].includes(name) &&
        !name.startsWith("_")
      ) {
        properties.set(name, type);
      }
      propMatch = propertyPattern.exec(text);
    }

    let eventMatch = eventPattern.exec(text);
    while (eventMatch) {
      const type = eventMatch[1].replace(/\s+/g, " ").trim();
      const name = eventMatch[2].trim();
      if (!name.startsWith("_")) {
        events.set(name, type);
      }
      eventMatch = eventPattern.exec(text);
    }
  }

  return {
    properties: [...properties.entries()]
      .sort((a, b) => a[0].localeCompare(b[0]))
      .map(([name, type]) => ({ name, type })),
    events: [...events.entries()]
      .sort((a, b) => a[0].localeCompare(b[0]))
      .map(([name, type]) => ({ name, type })),
  };
}

function toSlug(name) {
  return name
    .replace(/([a-z0-9])([A-Z])/g, "$1-$2")
    .replace(/_/g, "-")
    .toLowerCase();
}

function toControlTypeName(controlName) {
  if (controlName === "ListView") {
    return "ListView<T>";
  }
  if (controlName === "VirtualizedListView") {
    return "VirtualizedListView<T>";
  }
  if (controlName === "GroupedListView") {
    return "GroupedListView<TGroup, TItem>";
  }
  if (controlName === "DataForm") {
    return "DataForm<TModel>";
  }
  return controlName;
}

function buildUsageSample(controlName, properties) {
  const featured = featuredWidgetDocs.get(controlName);
  if (featured) {
    return featured.usage;
  }

  const typeName = toControlTypeName(controlName);
  const usesText = properties.some((x) => x.name === "Text");
  const usesTitle = properties.some((x) => x.name === "Title");
  const usesItems = properties.some((x) => x.name === "Items");

  const lines = [
    "using Tessera.Controls;",
    "using Tessera.Layout;",
    "",
    `var widget = new ${typeName}`,
    "{",
  ];

  if (usesText) {
    lines.push(`    Text = "${controlName}"`);
  } else if (usesTitle) {
    lines.push(`    Title = "${controlName}"`);
  } else if (usesItems) {
    lines.push("    // Configure Items here");
  } else {
    lines.push("    // Configure properties here");
  }

  lines.push("};");
  lines.push("");
  lines.push(
    "return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));",
  );
  return lines.join("\n");
}

function buildDefaultWhenToUse(controlName, family, members) {
  const usesItems = members.properties.some((x) => x.name === "Items");
  const usesTitle = members.properties.some((x) => x.name === "Title");
  const hasEvents = members.events.length > 0;

  const lines = [
    `You need a \`${controlName}\`-style interaction inside the ${family.toLowerCase()} lane.`,
  ];

  if (usesItems) {
    lines.push("The control manages an item collection and selection is part of the workflow.");
  }
  if (usesTitle) {
    lines.push("A titled widget surface improves scanability in dense shells.");
  }
  if (hasEvents) {
    lines.push("You want explicit user-driven events routed into app state updates.");
  } else {
    lines.push("The control is mainly presentational or state-driven through property updates.");
  }

  return lines;
}

function buildDefaultGotchas(controlName, family, members) {
  const hasEvents = members.events.length > 0;
  const hasFocusedStyle = members.properties.some((x) => x.name.includes("Focused"));
  const hasDisabledFlag = members.properties.some((x) => x.name === "IsDisabled");

  const lines = [
    `Do not choose \`${controlName}\` by name only; validate it against the target workflow.`,
    `Keep this control scoped to the ${family.toLowerCase()} concern; avoid cross-layer state coupling.`,
  ];

  if (hasEvents) {
    lines.push("Handle control events by posting/processing messages; avoid hidden mutation in render paths.");
  }
  if (hasFocusedStyle) {
    lines.push("Set focused/normal styles intentionally so keyboard focus remains obvious.");
  }
  if (hasDisabledFlag) {
    lines.push("Keep disabled state explicit and reversible so users understand why actions are blocked.");
  }

  return lines;
}

function buildWidgetDoc(control, members) {
  const typeName = toControlTypeName(control.className);
  const family = familyMap.get(control.className) ?? "Other";
  const slug = toSlug(control.className);
  const usage = buildUsageSample(control.className, members.properties);
  const featured = featuredWidgetDocs.get(control.className);
  const whenToUse = featured?.whenToUse ?? buildDefaultWhenToUse(control.className, family, members);
  const gotchas = featured?.gotchas ?? buildDefaultGotchas(control.className, family, members);
  const whenToUseSection =
    whenToUse.length === 0
      ? ""
      : `
## When to use

${whenToUse.map((x) => `- ${x}`).join("\n")}
`;
  const gotchasSection =
    gotchas.length === 0
      ? ""
      : `
## Common pitfalls

${gotchas.map((x) => `- ${x}`).join("\n")}
`;
  const propertiesTable =
    members.properties.length === 0
      ? "This control currently exposes no additional public properties beyond base `Control` members.\n"
      : [
          "| Property | Type |",
          "| --- | --- |",
          ...members.properties.map((x) => `| \`${x.name}\` | \`${x.type}\` |`),
        ].join("\n");
  const eventsTable =
    members.events.length === 0
      ? "This control currently exposes no public events.\n"
      : [
          "| Event | Type |",
          "| --- | --- |",
          ...members.events.map((x) => `| \`${x.name}\` | \`${x.type}\` |`),
        ].join("\n");

  return `---
title: "${typeName}"
sidebar_label: "${typeName}"
---

# \`${typeName}\`

**Family:** ${family}  
**Namespace:** \`Tessera.Controls\`

Use \`${typeName}\` when this interaction is the best match for your screen workflow.
${whenToUseSection}

## Minimal usage

\`\`\`csharp
${usage}
\`\`\`
${gotchasSection}

## Public properties

${propertiesTable}

## Public events

${eventsTable}

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
`;
}

function buildIndexDoc(controls) {
  const rows = controls.map((x) => {
    const typeName = toControlTypeName(x.className);
    const slug = toSlug(x.className);
    const family = familyMap.get(x.className) ?? "Other";
    return `| [\`${typeName}\`](/docs/widgets/${slug}) | ${family} |`;
  });
  const featuredRows = [...featuredWidgetDocs.keys()]
    .map((name) => {
      const typeName = toControlTypeName(name);
      const slug = toSlug(name);
      return `- [\`${typeName}\`](/docs/widgets/${slug})`;
    })
    .join("\n");

  return `---
title: Widget Pages
sidebar_label: Widget Pages
---

# Widget Pages

This section contains one page per public control in \`Tessera.Controls\`, with:

- a minimal usage sample
- public property list
- public event list

For discovery by product problem first, start with [Widget Reference](/docs/widget-reference).

## Featured first-read widgets

These pages include richer beginner-focused snippets and pitfall notes:

${featuredRows}

## All widget pages

| Widget | Family |
| --- | --- |
${rows.join("\n")}
`;
}

function writeFiles(controls) {
  fs.mkdirSync(outputDir, { recursive: true });

  const categoryJson = {
    label: "Widget Pages",
    position: 3,
    collapsed: true,
  };
  fs.writeFileSync(
    path.join(outputDir, "_category_.json"),
    `${JSON.stringify(categoryJson, null, 2)}\n`,
    "utf8",
  );

  fs.writeFileSync(path.join(outputDir, "index.md"), buildIndexDoc(controls), "utf8");

  for (const control of controls) {
    const members = extractPublicMembers(control);
    const slug = toSlug(control.className);
    const doc = buildWidgetDoc(control, members);
    fs.writeFileSync(path.join(outputDir, `${slug}.md`), doc, "utf8");
  }
}

function main() {
  const files = readFilesRecursive(controlsDir);
  const controls = findControlClasses(files);
  writeFiles(controls);
  console.log(`Generated ${controls.length} widget pages in docs/widgets.`);
}

main();
