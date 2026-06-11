class TableBuilder:
    def __init__(self, header: list, multiline_mode: bool = False):
        self.clen = len(header)
        self.column_widths = [0] * self.clen
        self.rowlists = []
        self.multiline_mode = multiline_mode
        self.add_row(header)


    def add_row(self, row: list):
        if len(row) < self.clen:
            row = row + [""] * (self.clen - len(row))
        
        for j in range(self.clen):
            lines = str(row[j]).split('\n')
            max_line_len = max((len(line) for line in lines), default=0)
            self.column_widths[j] = max(self.column_widths[j], max_line_len)
        
        self.rowlists.append(row)


    def build(self) -> str:
        result = [
            self._format_outside_sep(),
            self._format_row(self.rowlists[0])
        ]
        
        if self.multiline_mode:
            result.append(self._format_inside_sep('-'))
            for i, row in enumerate(self.rowlists[1:]):
                result.append(self._format_row(row))
                if i < len(self.rowlists[1:]) - 1:
                    result.append(self._format_inside_sep('-'))
        else:
            result.append(self._format_inside_sep('-'))
            for row in self.rowlists[1:]:
                result.append(self._format_row(row))

        result.append(self._format_outside_sep())
        return "\n".join(result)
    

    def _format_inside_sep(self, char: str = '-') -> str:
        base_sep = "| " + " | ".join([char * self.column_widths[j] for j in range(self.clen)]) + " |"
        return base_sep.replace(" ", char)


    def _format_outside_sep(self) -> str:
        total_inner_width = sum(self.column_widths) + 3 * self.clen - 1
        return "+" + ("-" * total_inner_width) + "+"


    def _format_row(self, row: list) -> str:
        cell_lines = [str(row[j]).split('\n') for j in range(self.clen)]
        max_height = max((len(lines) for lines in cell_lines), default=1)
        
        formatted_subrows = []
        for i in range(max_height):
            subrow = []
            for j in range(self.clen):
                val = cell_lines[j][i] if i < len(cell_lines[j]) else ""
                subrow.append(val.ljust(self.column_widths[j]))
            formatted_subrows.append("| " + " | ".join(subrow) + " |")
            
        return "\n".join(formatted_subrows)