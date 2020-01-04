QUEST_SCE_DAILYBOOK2 = {
	title = 'IDS_PROPQUEST_SCENARIO_INC_000501',
	character = 'MaFl_SgRadion',
	end_character = 'MaDa_DarMayor',
	start_requirements = {
		min_level = 105,
		max_level = 129,
		job = { 'JOB_VAGRANT', 'JOB_MERCENARY', 'JOB_ACROBAT', 'JOB_ASSIST', 'JOB_MAGICIAN', 'JOB_KNIGHT', 'JOB_BLADE', 'JOB_JESTER', 'JOB_RANGER', 'JOB_RINGMASTER', 'JOB_BILLPOSTER', 'JOB_PSYCHIKEEPER', 'JOB_ELEMENTOR', 'JOB_KNIGHT_MASTER', 'JOB_BLADE_MASTER', 'JOB_JESTER_MASTER', 'JOB_RANGER_MASTER', 'JOB_RINGMASTER_MASTER', 'JOB_BILLPOSTER_MASTER', 'JOB_PSYCHIKEEPER_MASTER', 'JOB_ELEMENTOR_MASTER', 'JOB_KNIGHT_HERO', 'JOB_BLADE_HERO', 'JOB_JESTER_HERO', 'JOB_RANGER_HERO', 'JOB_RINGMASTER_HERO', 'JOB_BILLPOSTER_HERO', 'JOB_PSYCHIKEEPER_HERO', 'JOB_ELEMENTOR_HERO' },
		previous_quest = 'QUEST_SCE_DAILYBOOK1',
	},
	rewards = {
		gold = 142000,
		exp = 23764461,
	},
	end_conditions = {
	},
	dialogs = {
		begin = {
			'IDS_PROPQUEST_SCENARIO_INC_000502',
			'IDS_PROPQUEST_SCENARIO_INC_000503',
		},
		begin_yes = {
			'IDS_PROPQUEST_SCENARIO_INC_000504',
		},
		begin_no = {
			'IDS_PROPQUEST_SCENARIO_INC_000505',
		},
		completed = {
			'IDS_PROPQUEST_SCENARIO_INC_000506',
			'IDS_PROPQUEST_SCENARIO_INC_000507',
		},
		not_finished = {
			'IDS_PROPQUEST_SCENARIO_INC_000787',
		},
	}
}
