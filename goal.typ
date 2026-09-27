// Use Typst to compile me into pdf: https://typst.app
// I can also recommend a VSCode extension, but use whatever you want, this thing literally runs anywhere and is more powerful than LaTeX.

#set page(
  paper: "a4",
  margin: 2.2cm,
)

#set text(
  size: 12pt,
  fill: black,
  lang: "en",
)

#show figure.caption: set text(size: 0.8em)
#show bibliography: set heading(level: 2)

// It may look a little intimidating, but trust me, after you set all the formats you can write papers pretty much in Markdown.
// Many typst docs consist of a 50-line type setter preamble followed by a couple thousand lines of pretty much plain text, no TeX BS needed.
#grid(columns: (1fr, 1fr),
  [
    #set text(
      size: 14pt,
    )

    #set align(center)

    #heading()[
      How Close to the Human Should I Get: AI Decision Making in Collaborative Task-Based Games
    ]

    #v(0.5em)

    AI playing games
  ], [
    #set align(right)
    #show: box.with()

    #set text(
      size: 10pt,
      fill: luma(60%),
    )

    #align(left)[
      *Authors:*
      - Ilia Kudriashov
      - Imbert Dam
      - Manu van Vlerken
      - Motaz Motaz A.J. Abumandil
      - Renkai Ma
      - Thomas van Egmond
    ]
  ]
)

== Goal

There is some research going into agentic AI behavior in collaborative real-time task-based games such as Overcooked @trust-and-collaboration-in-human-autonomy-teams.
Our team is interested in exploring how different degrees of AI "closeness" to the human player affect perceived likability of the AI agent.
In particular, we want to explore how two different adaptive task scheduling algorithms illustrated in @fig:distant-close-ai affect AI likability and perceived competence when implemented into an autonomous agent in a collaborative game.

#figure(
  image("ai-behavior-diagram.png"),
  caption:[
    An illustration showing the two different adaptive task scheduling algorithms we plan to implement into an autonomous agent in our collaborative game.
  ],
) <fig:distant-close-ai>

== Approach

We plan on implementing two different adaptive task scheduling algorithms into an autonomous agent in a collaborative game similar in its core concepts to Overcooked. 
We will then conduct user studies to evaluate the perceived likability and competence of the AI agent with different task scheduling algorithms and in scenarios with different task dependencies.

Similar studies have been conducted exploring different versions of collaborative AI agents in dynamic physics-based environments @should-i-lead-or-follow, and we plan on copying their approach to user studies with minor changes.

== Data collection & evaluation

The main data we are interested in collecting is the perceived likability and effectiveness of the two AI agents with different task scheduling algorithms.
We aim to compare the two approaches between each other, so both data collection and evaluation practices should be designed with the goal of comparing the two in mind.
We are yet to decide the exact study approach, but we are considering a within-subjects design where each participant will interact with both AI agents in different scenarios. We will then use the aggregate data collected through after-play surveys to evaluate comparative likability and effectiveness, which would, in theory, allow us to make conclusions about which AI is more likable in the context of the game we constructed.

#bibliography("literature.bib")
