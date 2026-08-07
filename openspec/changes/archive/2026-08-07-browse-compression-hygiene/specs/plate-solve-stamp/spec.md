## MODIFIED Requirements

### Requirement: Solve input paths
An uncompressed XISF SHALL be handed to the solver directly. A compressed XISF SHALL be
surgically rewritten as a temporary **uncompressed XISF** — XML preserved except the block
attributes the decompression forces, block decompressed, no pixel re-encoding and no
intermediate FITS — and that temporary file handed to the solver; the solver consumes one input
format (XISF) for all cases. The rewrite SHALL NOT restrict the frame's sample format or channel
count beyond what the solver itself supports. All temporary solve artifacts (input copy and
solver output files) SHALL be created outside the image library and removed afterward, and
solver output SHALL be redirected so no solver file ever appears next to a library image.

#### Scenario: Compressed backlog frame solves
- **WHEN** a zlib+shuffle-compressed light frame is read with the solver checked
- **THEN** it solves via a temporary uncompressed XISF and the original file's directory gains
  no new files

#### Scenario: No FITS intermediate
- **WHEN** any frame is solved
- **THEN** no FITS file is produced by the application as solver input
