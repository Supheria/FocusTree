focus_tree = 
{
	id = ##focus id
	country = 
	##country block
	{
		factor = 0
		modifier = 
		{
			add = 20
			tag | original_tag = ##country tag
		}
	}
	default = no | yes
	continuous_focus_position = { x = (int) y = (int) } ##循环国策面板 left&top（黑色矩形面板）
	shared_focus = ##focus tag
}