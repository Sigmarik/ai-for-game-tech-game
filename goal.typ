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

// It may look a little intimidating, but trust me, after you set all the formats you can write papers pretty much in Markdown.
// Many typst docs consist of a 50-line type setter preamble followed by a couple thousand lines of pretty much plain text, no TeX BS needed.
#grid(columns: (1fr, 1fr),
  [
    #set text(
      size: 14pt,
    )

    #set align(center)

    #heading()[
      Project Description:\
      Group 1
    ]

    #v(1em)

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

Here is a shit ton of references to related and semi-related papers
@should-i-lead-or-follow @trust-and-collaboration-in-human-autonomy-teams @likable-and-competent-ai-sidekick @follow-my-lead @dynamic-game-difficulty-scaling-using-adaptive-behavior-based-ai @promoting-human-ai-interaction which someone wrote before us.
I am a little lazy to analyze them, at least for now, but I will be able to finish the job after a short rest and be done with the assignment.

#lorem(100)

#bibliography("literature.bib")
