# Markdown User Stories

A simple, local-only web app to create and display user stories, and store them as Markdown files.

## Features

[Shortest User Story Intro Ever \| Software Meadows](https://www.softwaremeadows.com/posts/shortest_user_story_intro_ever/)

How long would it take to create a simple user story app? All it needs to do is capture and display:

*	Who, What, Why (as a ___ I want ___ so that ____)
*	Discussion
*	Acceptance Criteria
*	Status

Nice to have
*	CreatedOn, StartedOn, CompletedOn

And for fun, let's store each story in Markdown instead of a database.

```markdown
---
CreatedOn:    20190523-12:15
# Use this initially, it's ISO standard
StartedOn:    2019-05-24T09:00  
CompletedOn:  2019-05-24 16:33
# Backlog, In Process, Waiting On, Done
Status:       Done
Sequence:     5
Estimate:     XL
---

# As a {ROLE}, {WANT} {WHY}

## Discussion
{DISCUSSION}

## Acceptance Criteria
{ACCEPTANCE CRITERIA}

```

## Nice to Have

*	Attachments
*	Tasks with time estimates
*	CreatedBy, AssignedTo
*	Authentication


