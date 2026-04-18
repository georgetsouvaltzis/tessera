import * as React from 'react';
import { Slot } from '@radix-ui/react-slot';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '@site/src/lib/utils';

const buttonVariants = cva(
  'inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-[0.9rem] text-sm font-semibold transition-all outline-none focus-visible:ring-2 focus-visible:ring-[var(--ifm-color-primary)] disabled:pointer-events-none disabled:opacity-50',
  {
    variants: {
      variant: {
        primary:
          'border border-white/10 bg-[var(--tessera-gradient-primary)] text-[#140712] shadow-[0_16px_38px_rgba(255,70,165,0.24),inset_0_1px_0_rgba(255,255,255,0.28)] hover:-translate-y-px hover:shadow-[0_18px_44px_rgba(255,70,165,0.32),inset_0_1px_0_rgba(255,255,255,0.32)]',
        secondary:
          'border border-[var(--tessera-border-strong)] bg-white/3 text-[var(--tessera-text)] hover:-translate-y-px hover:border-[#ff7fc5]/35 hover:bg-[#ff7fc5]/8',
        ghost:
          'text-[var(--tessera-text-muted)] hover:-translate-y-px hover:text-[var(--tessera-text)]',
      },
      size: {
        sm: 'h-9 px-3.5',
        md: 'h-10 px-4',
        lg: 'h-11 px-5 text-[0.95rem]',
      },
    },
    defaultVariants: {
      variant: 'primary',
      size: 'md',
    },
  },
);

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean;
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant, size, asChild = false, ...props }, ref) => {
    const Comp = asChild ? Slot : 'button';

    return (
      <Comp
        className={cn(buttonVariants({ variant, size }), className)}
        ref={ref}
        {...props}
      />
    );
  },
);

Button.displayName = 'Button';

export { Button, buttonVariants };
