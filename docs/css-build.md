# Building the site CSS

From the directory containing `package.json` and `tailwind.config.js`:

```sh
npm ci
npm run css:build
```

Use the colon in `css:build`. `npm run css` starts the long-running watcher.

## Why `bg-canvas` can be reported as missing

`bg-canvas`, `text-ink`, and the site's other theme classes are defined in the
root `tailwind.config.js`; they are not default Tailwind classes. If an editor
or CLI invocation starts in another working directory and does not load this
config, an `@apply bg-canvas` rule fails with `CssSyntaxError`.

The build now anchors both configuration and content discovery:

- `app.css` declares `@config "../../../../../tailwind.config.js"`, resolved
  relative to the stylesheet. Preserve the checkout's folder structure.
- The npm build/watch scripts also pass `--config ./tailwind.config.js`.
- `content.relative: true` resolves Razor/HTML/JS globs relative to the config,
  so alternate working directories do not silently omit template utilities.

Pull all changed files, including the root configuration and `package.json`,
not just `app.css`. The project uses the locked Tailwind 3 dependency; a
Tailwind 4 migration or dependency update is not needed for this correction.

## Regression check

```sh
npm run css:test
```

The Node built-in test runner builds the actual stylesheet from the repository
root, Web project, and a temporary directory whose name contains spaces. It
asserts that semantic theme classes and recent components/template utilities
are present and that all three outputs are identical. Outputs are temporary
and cleaned up. This runs real Tailwind builds, not a simulated CSS fixture.

The regression check and npm builds from both root/Web directories passed in
the Linux implementation environment. A native Windows execution was not
available; test subprocess arguments use Node's argument arrays and path APIs
rather than shell-specific quoting.

The outdated Browserslist database warning is separate and non-fatal. If the
same error persists after pulling, confirm the referenced root config exists
and run `npm ci` to restore the lockfile's dependency versions before retrying.
