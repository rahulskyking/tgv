const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');
const postcss = require('postcss');

const root = path.resolve(__dirname, '..');
const web = path.join(root, 'src', 'Web', 'TheGameVoice.Web');
const input = path.join(web, 'wwwroot', 'css', 'app.css');
const cli = require.resolve('tailwindcss/lib/cli.js');

// No shell interpolation: node and path.join handle Windows paths/spaces too.
// Deliberately omit --config to exercise app.css's explicit @config directive.
test('Tailwind loads the same theme and templates from any working directory', () => {
    const temp = fs.mkdtempSync(path.join(os.tmpdir(), 'tgv css build '));
    try {
        let expected;
        for (const [index, cwd] of [root, web, temp].entries()) {
            const output = path.join(temp, `site-${index}.css`);
            const result = spawnSync(process.execPath, [cli, '-i', input, '-o', output, '--minify'], {
                cwd, encoding: 'utf8', timeout: 60000
            });
            assert.ifError(result.error);
            assert.equal(result.status, 0, `Tailwind failed from ${cwd}:\n${result.stderr}`);
            const css = fs.readFileSync(output, 'utf8');
            const selectors = new Set();
            postcss.parse(css).walkRules(rule => rule.selectors.forEach(selector => selectors.add(selector)));
            for (const selector of ['.bg-canvas', '.text-ink', '.tgv-tag-row', '.tgv-review-score', '.grid-cols-3']) {
                assert.ok(selectors.has(selector), `Missing ${selector} when built from ${cwd}`);
            }
            if (expected === undefined) expected = css;
            else assert.equal(css, expected, `Working directory changed generated CSS: ${cwd}`);
        }
    } finally {
        fs.rmSync(temp, { recursive: true, force: true });
    }
});
