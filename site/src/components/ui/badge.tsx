import * as React from 'react';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '@site/src/lib/utils';

const badgeVariants = cva(
  'inline-flex w-fit items-center rounded-full border px-3 py-1 text-[0.72rem] font-bold uppercase tracking-[0.08em]',
  {
    variants: {
      variant: {
        default: 'border-[#ff7fc5]/20 bg-[#ff7fc5]/8 text-[#ffbade]',
        muted: 'border-white/10 bg-white/4 text-[var(--tessera-text-muted)]',
      },
    },
    defaultVariants: {
      variant: 'default',
    },
  },
);

export interface BadgeProps
  extends React.HTMLAttributes<HTMLDivElement>,
    VariantProps<typeof badgeVariants> {}

export function Badge({ className, variant, ...props }: BadgeProps): React.JSX.Element {
  return <div className={cn(badgeVariants({ variant }), className)} {...props} />;
}
