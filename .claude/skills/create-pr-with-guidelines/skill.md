---
name: create-pr-with-guidelines
description: Create a descriptive pull request from the current code changes.
version: 1.0.0
tags:
  - git
  - github
  - pull-request
---

# Create PR With Guidelines

Use this workflow when creating a pull request for repository changes.

## Workflow

1. Inspect the worktree and identify the base branch and remote.
2. Create a descriptive feature branch unless the current branch is already
   dedicated to the change.
3. Review the diff and run the smallest relevant validation.
4. Commit all requested changes with a concise, descriptive commit message.
5. Push the branch and set its upstream.
6. Create the pull request against the repository's default branch.

## Pull request guidelines

- Use a title that describes the user-visible change, for example:
  `Add Prototype design pattern C++ example`.
- The PR description must include the complete input prompt that requested the
  change. Prefer embedding the checked-in `prompt.md` content under an
  `## Input prompt` heading.
- Summarize the implementation and validation results.
- Do not claim tests passed unless they were actually run.
- Link the created PR in the final response.

## Required prompt artifact

For every change, check in the original request as `prompt.md` in the folder
where the requested code changes are made. Keep the prompt faithful to the
user's wording while formatting it as readable Markdown.
