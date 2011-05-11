using System;
using System.Web.Mvc;
using NUnit.Framework;
using SpecsFor;
using TryCatchFail.CodeStock2011.FailTracker.Core.Data;
using TryCatchFail.CodeStock2011.FailTracker.Core.Domain;
using TryCatchFail.CodeStock2011.FailTracker.Web.Controllers;
using MvcContrib.TestHelper;
using TryCatchFail.CodeStock2011.FailTracker.Web.Models.Issues;
using Should;
using System.Linq;

namespace TryCatchFail.CodeStock2011.UnitTests.Web.Controllers
{
	public class IssueControllerSpecs
	{
		public class when_requesting_the_list_of_issues : SpecsFor<IssuesController>
		{
			private ActionResult _result;

			protected override void Given()
			{
				GetMockFor<IRepository<Issue>>()
					.Setup(s => s.Query())
					.Returns((new[] {Issue.Create("Test1", "blah", "blah")}).AsQueryable());
			}

			protected override void When()
			{
				_result = SUT.Dashboard();
			}

			[Test]
			public void then_it_returns_a_view()
			{
				_result.AssertViewRendered().WithViewData<IssueViewModel[]>().ShouldNotBeEmpty();
			}
		}
		
		public class when_adding_a_new_issue : SpecsFor<IssuesController>
		{
			private ActionResult _result;
			private readonly Guid TestIssueID = Guid.NewGuid();

			protected override void Given()
			{
				GetMockFor<IRepository<Issue>>()
					.Setup(s => s.Save(Issue.Create("Test Title", "Matt", "Content")))
					.Callback<Object>(i => ((Issue) i).ID = TestIssueID)
					.Verifiable();
			}

			protected override void When()
			{
				_result = SUT.AddIssue(new AddIssueForm {AssignedTo = "Matt", Title = "Test Title", Body = "Content"});
			}

			[Test]
			public void then_it_will_send_an_add_issue_command()
			{
				GetMockFor<IRepository<Issue>>().Verify();
			}

			[Test]
			public void then_it_returns_a_redirect_to_the_view_issue_action_for_the_new_issue()
			{
				_result.AssertActionRedirect().ToAction<IssuesController>(c => c.View(TestIssueID));
			}
		}
	}
}