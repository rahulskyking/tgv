/** @type {import('tailwindcss').Config} */

/*  TheGameVoice design system.

    Everything visual is declared here or in wwwroot/css/app.css and compiled
    into wwwroot/css/site.css by `npm run css:build`. There are no hand-written
    stylesheets loaded next to the build any more.

    Colours are exposed as `R G B` channel triplets in CSS variables so that
    Tailwind's opacity modifiers keep working (bg-primary/10, text-ink/70),
    and so that switching to dark mode is a variable swap — no `dark:` variant
    needed on the elements that use the semantic colours.
*/

const withAlpha = (variable) => `rgb(var(${variable}) / <alpha-value>)`;

module.exports = {
    darkMode: ['selector', '[data-theme="dark"]'],

    content: [
        "./src/Web/TheGameVoice.Web/**/*.cshtml",
        "./src/Web/TheGameVoice.Web/**/*.html",
        "./src/Web/TheGameVoice.Web/**/*.js"
    ],

    theme: {
        screens: {
            /* 400px catches the 320-390px phones that were breaking the header */
            'xs': '400px',
            'sm': '640px',
            'md': '768px',
            'lg': '1024px',
            'xl': '1280px',
            '2xl': '1536px',
            /* large desktop / TV */
            '3xl': '1920px'
        },

        extend: {
            colors: {
                /* Brand */
                primary: {
                    DEFAULT: withAlpha('--tgv-primary'),
                    hover: withAlpha('--tgv-primary-hover')
                },

                /* Page background */
                canvas: withAlpha('--tgv-background'),

                /* Cards and raised areas */
                surface: {
                    DEFAULT: withAlpha('--tgv-surface'),
                    muted: withAlpha('--tgv-surface-2'),
                    raised: withAlpha('--tgv-surface-3')
                },

                /* Text */
                ink: {
                    DEFAULT: withAlpha('--tgv-text'),
                    soft: withAlpha('--tgv-text-soft'),
                    mid: withAlpha('--tgv-text-mid'),
                    muted: withAlpha('--tgv-text-muted'),
                    faint: withAlpha('--tgv-text-faint')
                },

                /* `text-muted` is used widely in the views already */
                muted: withAlpha('--tgv-text-muted'),

                /* Borders. `border-default` is used widely in the views. */
                line: {
                    DEFAULT: withAlpha('--tgv-border'),
                    subtle: withAlpha('--tgv-border-subtle'),
                    strong: withAlpha('--tgv-border-strong')
                },
                default: withAlpha('--tgv-border')
            },

            fontFamily: {
                sans: ['Inter', 'system-ui', 'sans-serif'],
                display: ['"Barlow Condensed"', 'sans-serif'],
                alt: ['"Space Grotesk"', 'sans-serif']
            },

            /*  Fluid type: one class, correct on a 320px phone and a 4K panel.
                Replaces stacks like `text-3xl sm:text-5xl lg:text-7xl`.        */
            fontSize: {
                'fluid-display': ['clamp(2.25rem, 1.35rem + 4vw, 4.5rem)', { lineHeight: '1.02', letterSpacing: '-0.03em' }],
                'fluid-h1': ['clamp(1.875rem, 1.2rem + 2.9vw, 3.5rem)', { lineHeight: '1.06', letterSpacing: '-0.03em' }],
                'fluid-h2': ['clamp(1.5rem, 1.16rem + 1.5vw, 2.25rem)', { lineHeight: '1.15', letterSpacing: '-0.025em' }],
                'fluid-h3': ['clamp(1.25rem, 1.14rem + .5vw, 1.5rem)', { lineHeight: '1.25' }],
                'fluid-lead': ['clamp(1rem, .93rem + .32vw, 1.25rem)', { lineHeight: '1.65' }],
                'fluid-body': ['clamp(1rem, .955rem + .22vw, 1.15rem)', { lineHeight: '1.8' }],

                /*  Fluid mirrors of Tailwind's own scale: same maximum as
                    text-3xl … text-7xl, but they shrink on small screens
                    instead of overflowing a 360px phone.                     */
                'fluid-3xl': ['clamp(1.5rem, 1.31rem + .84vw, 1.875rem)', { lineHeight: '1.2' }],
                'fluid-4xl': ['clamp(1.75rem, 1.43rem + 1.4vw, 2.25rem)', { lineHeight: '1.15' }],
                'fluid-5xl': ['clamp(2rem, 1.5rem + 2.2vw, 3rem)', { lineHeight: '1.1' }],
                'fluid-6xl': ['clamp(2.25rem, 1.6rem + 2.9vw, 3.75rem)', { lineHeight: '1.05' }],
                'fluid-7xl': ['clamp(2.5rem, 1.66rem + 3.75vw, 4.5rem)', { lineHeight: '1' }]
            },

            /* Fluid section rhythm: `py-section` instead of `py-24` */
            spacing: {
                'section': 'clamp(2.5rem, 1.5rem + 4.5vw, 6rem)',
                'section-sm': 'clamp(1.75rem, 1.1rem + 2.9vw, 4rem)',
                'gutter': 'clamp(16px, 4vw, 24px)',

                /*  Fluid mirrors of the spacing steps the views use without a
                    breakpoint prefix. Same desktop maximum, smaller on phones. */
                'fluid-28': 'clamp(2.75rem, 1.5rem + 5.6vw, 7rem)',
                'fluid-24': 'clamp(2.5rem, 1.4rem + 5vw, 6rem)',
                'fluid-20': 'clamp(2.25rem, 1.3rem + 4.2vw, 5rem)',
                'fluid-16': 'clamp(2rem, 1.25rem + 3.4vw, 4rem)',
                'fluid-14': 'clamp(1.875rem, 1.2rem + 3vw, 3.5rem)',
                'fluid-12': 'clamp(1.75rem, 1.2rem + 2.4vw, 3rem)',
                'fluid-10': 'clamp(1.5rem, 1.1rem + 1.9vw, 2.5rem)',
                'fluid-8': 'clamp(1.25rem, 1rem + 1.2vw, 2rem)'
            },

            maxWidth: {
                container: 'var(--tgv-container-width)',
                prose: '72ch'
            },

            borderRadius: {
                tgv: 'var(--tgv-radius-md)',
                'tgv-lg': 'var(--tgv-radius-lg)',
                'tgv-xl': 'var(--tgv-radius-xl)'
            },

            boxShadow: {
                card: 'var(--tgv-shadow)'
            },

            transitionProperty: {
                theme: 'background-color, border-color, color, fill'
            }
        }
    },

    plugins: []
};
