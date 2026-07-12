# AI knowledge package provenance

- Source repository: `PVF-Agent-Workbench`
- Source commit: `41fc9f566bcfc08ed1f93d33fd7eb9470194e85c`
- Source allowlist: `knowledge-pack/MANIFEST.json` at that commit
- Manifest entries copied: 273
- Additional license copied: `LICENSE-KNOWLEDGE-CC0.md`
- Packaged files: 274
- Current raw size: 4,339,466 bytes
- Canonical tree SHA-256: `5973788a44fa7a2a4cb6c6f68b6a56d56a965a891783cf8e7034c1f0a1725ae0`

Every copied manifest file matches its declared per-file SHA-256. The sum of the
273 entry-level `bytes` values is 4,338,910. The manifest's own summary reports
4,338,430 bytes, which is 480 bytes lower than its entry-level data; this project
records the discrepancy instead of treating that summary field as authoritative.

The canonical tree hash is calculated over all 274 packaged files in ordinal
relative-path order. For each file, the hash input is the UTF-8 relative path,
one NUL byte, the file bytes with CRLF and CR normalized to LF, and one final NUL
byte. This makes provenance checks insensitive to checkout line-ending policy
while still detecting path, content, addition, and removal changes.

The source worktree contained uncommitted experimental material. Only the clean
commit allowlist was migrated. In particular, dirty-only task cards and
workflows, `.env` files, real PVFs, local profiles, generated runtime output,
Node/native runtimes, and deployment automation are not part of this package.
