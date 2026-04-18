import * as React from 'react';
import { Slot } from '@radix-ui/react-slot';
import { cn } from '@site/src/lib/utils';

export interface SurfaceCardProps extends React.HTMLAttributes<HTMLDivElement> {
  asChild?: boolean;
}

export const SurfaceCard = React.forwardRef<HTMLDivElement, SurfaceCardProps>(
  ({ className, asChild = false, ...props }, ref) => {
    const Comp = asChild ? Slot : 'div';

    return (
      <Comp
        className={cn(
          'lumina-panel ui-surface-card',
          className,
        )}
        ref={ref}
        {...props}
      />
    );
  },
);

SurfaceCard.displayName = 'SurfaceCard';
