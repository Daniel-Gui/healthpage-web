<script lang="ts">
	import HeartHandshake from "@lucide/svelte/icons/heart-handshake";
	import CameraIcon from "@tabler/icons-svelte/icons/camera";
	import ChartBarIcon from "@tabler/icons-svelte/icons/chart-bar";
	import DashboardIcon from "@tabler/icons-svelte/icons/dashboard";
	import DatabaseIcon from "@tabler/icons-svelte/icons/database";
	import FileAiIcon from "@tabler/icons-svelte/icons/file-ai";
	import FileDescriptionIcon from "@tabler/icons-svelte/icons/file-description";
	import FileWordIcon from "@tabler/icons-svelte/icons/file-word";
	import FolderIcon from "@tabler/icons-svelte/icons/folder";
	import HelpIcon from "@tabler/icons-svelte/icons/help";
	import InnerShadowTopIcon from "@tabler/icons-svelte/icons/inner-shadow-top";
	import ListDetailsIcon from "@tabler/icons-svelte/icons/list-details";
	import ReportIcon from "@tabler/icons-svelte/icons/report";
	import SearchIcon from "@tabler/icons-svelte/icons/search";
	import SettingsIcon from "@tabler/icons-svelte/icons/settings";
	import UsersIcon from "@tabler/icons-svelte/icons/users";
	import NavDocuments from "./nav-documents.svelte";
	import NavMain from "./nav-main.svelte";
	import NavSecondary from "./nav-secondary.svelte";
	import NavUser from "./nav-user.svelte";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import type { ComponentProps } from "svelte";

	const data = {
		user: {
			name: "Nome do profissional",
			email: "profissional@exemplo.com",
			avatar: "https://blog.sinaxys.com/wp-content/uploads/2024/08/mercado-de-trabalho-da-medicina-medico-sorrindo.jpg",
		},
		navMain: [
			{
				title: "Dashboard",
				url: "#",
				icon: DashboardIcon,
			},
			{
				title: "Website",
				url: "#",
				icon: ListDetailsIcon,
			},
			{
				title: "Agendamentos",
				url: "#",
				icon: ChartBarIcon,
			},
			{
				title: "Leads",
				url: "#",
				icon: ChartBarIcon,
			},
			{
				title: "Feedbacks",
				url: "#",
				icon: ChartBarIcon,
			},
		],
		navSecondary: [
			{
				title: "Assinar o Premium",
				url: "#",
				icon: SettingsIcon,
			},
			{
				title: "Suporte",
				url: "#",
				icon: HelpIcon,
			},
			{
				title: "Configurações",
				url: "#",
				icon: SettingsIcon,
			}
		],
		documents: [
			{
				name: "Item 1",
				url: "#",
				icon: DatabaseIcon,
			},
			{
				name: "Item 2",
				url: "#",
				icon: ReportIcon,
			},
			{
				name: "Item 3",
				url: "#",
				icon: FileWordIcon,
			},
		],
	};

	let { ...restProps }: ComponentProps<typeof Sidebar.Root> = $props();
</script>

<Sidebar.Root collapsible="offcanvas" {...restProps}>
	<Sidebar.Header>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton class="data-[slot=sidebar-menu-button]:!p-1.5">
					{#snippet child({ props })}
						<a href="##" {...props}>
							<HeartHandshake class="!size-5" />
							<span class="text-base font-semibold">HealthPage</span>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.navMain} />
		<NavDocuments items={data.documents} />
		<NavSecondary items={data.navSecondary} class="mt-auto" />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
</Sidebar.Root>
